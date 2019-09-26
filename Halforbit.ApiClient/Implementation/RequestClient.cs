using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Halforbit.ApiClient
{
    public class RequestClient : IRequestClient
    {
        static readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false
            })
        {
            Timeout = Timeout.InfiniteTimeSpan
        };

        public static IRequestClient Instance => new RequestClient();

        public async Task<Response> ExecuteAsync(Request request)
        {
            var services = request.Services;

            var reauthorizeRetriesRemaining = 1;

            var failRetriesRemaining = services.RetryStrategy?.RetryCount ?? 0;

            var failRetryCount = 0;
            
            while(true)
            {
                if(services.AuthorizationStrategy != null)
                {
                    request = await services.AuthorizationStrategy.Apply(request);
                }

                var requestUrl = !string.IsNullOrWhiteSpace(request.BaseUrl) ? 
                    $"{request.BaseUrl}/{request.Resource}" :
                    request.Resource;

                foreach (var kv in request.RouteValues)
                {
                    requestUrl = requestUrl.Replace($"{{{kv.Key}}}", kv.Value);
                }

                if(request.QueryValues.Count > 0)
                {
                    var query = HttpUtility.ParseQueryString(string.Empty);

                    foreach(var kv in request.QueryValues)
                    {
                        query[kv.Key] = kv.Value;
                    }

                    requestUrl = $"{requestUrl}?{query}";
                }

                (request, requestUrl) = await ApplyBeforeRequestHandlers(
                    services.BeforeRequestHandlers, 
                    request, 
                    requestUrl);

                var httpRequestMessage = new HttpRequestMessage(
                    method: new HttpMethod(request.Method),
                    requestUri: requestUrl);

                if(request.Headers.Count > 0)
                {
                    foreach(var kv in request.Headers)
                    {
                        httpRequestMessage.Headers.Add(kv.Key, kv.Value);
                    }
                }

                if (request.Content != null)
                {
                    var requestContent = new StreamContent(request.Content.GetStream());

                    if(!string.IsNullOrWhiteSpace(request.ContentType?.Value))
                    {
                        requestContent.Headers.Add("Content-Type", request.ContentType.Value);
                    }

                    httpRequestMessage.Content = requestContent;
                }

                var httpResponseMessage = default(HttpResponseMessage);

                try
                {
                    var cancellationTokenSource = new CancellationTokenSource();

                    var timeoutTask = Task.Delay(
                        request.Timeout.TotalSeconds > 0 ? 
                            request.Timeout : 
                            Timeout.InfiniteTimeSpan,
                        cancellationTokenSource.Token);

                    var sendTask = _httpClient.SendAsync(
                        httpRequestMessage, 
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationTokenSource.Token);

                    var finishedTask = await Task.WhenAny(sendTask, timeoutTask);

                    cancellationTokenSource.Cancel();

                    if(finishedTask == timeoutTask)
                    {
                        if ((services.RetryStrategy?.ShouldRetryOnTimeout ?? false) && 
                            failRetriesRemaining > 0)
                        {
                            var shouldRetry = true;

                            (request, requestUrl, shouldRetry) = await ApplyBeforeRetryHandlers(
                                request.Services.BeforeRetryHandlers,
                                request,
                                requestUrl,
                                0,
                                failRetryCount);

                            if(!shouldRetry)
                            {
                                return await ApplyAfterResponseHandlers(
                                    services.AfterResponseHandlers,
                                    request,
                                    new Response(
                                        statusCode: default,
                                        headers: default,
                                        content: default,
                                        contentType: default,
                                        contentEncoding: default,
                                        isSuccess: false,
                                        errorMessage: "Timed out while making request",
                                        exception: default,
                                        request: request,
                                        requestedUrl: requestUrl));
                            }

                            failRetriesRemaining--;

                            failRetryCount++;

                            var retryInterval = services.RetryStrategy.GetRetryTimeout(failRetryCount);

                            if (retryInterval.TotalSeconds > 0)
                            {
                                await Task.Delay(retryInterval);
                            }

                            continue;
                        }
                        else
                        {
                            return await ApplyAfterResponseHandlers(
                                services.AfterResponseHandlers, 
                                request, 
                                new Response(
                                    statusCode: default,
                                    headers: default,
                                    content: default,
                                    contentType: default,
                                    contentEncoding: default,
                                    isSuccess: false,
                                    errorMessage: "Timed out while making request",
                                    exception: default,
                                    request: request,
                                    requestedUrl: requestUrl));
                        }
                    }
                    else
                    {
                        httpResponseMessage = await sendTask;
                    }
                }
                catch(Exception ex)
                {
                    return await ApplyAfterResponseHandlers(
                        services.AfterResponseHandlers,
                        request,
                        new Response(
                            statusCode: default,
                            headers: default,
                            content: default,
                            contentType: default,
                            contentEncoding: default,
                            isSuccess: false,
                            errorMessage: ex.Message,
                            exception: ex,
                            request: request,
                            requestedUrl: requestUrl));
                }

                if ((services.AuthorizationStrategy?.ShouldReauthorize(httpResponseMessage.StatusCode) ?? false) && 
                    reauthorizeRetriesRemaining > 0)
                {
                    await services.AuthorizationStrategy.Reauthorize();

                    reauthorizeRetriesRemaining--;

                    continue;
                }

                if ((services.RetryStrategy?.ShouldRetry(httpResponseMessage.StatusCode) ?? false) && 
                    failRetriesRemaining > 0)
                {
                    var shouldRetry = true;

                    (request, requestUrl, shouldRetry) = await ApplyBeforeRetryHandlers(
                        request.Services.BeforeRetryHandlers, 
                        request, 
                        requestUrl, 
                        httpResponseMessage.StatusCode, 
                        failRetryCount);

                    if (shouldRetry)
                    {
                        failRetriesRemaining--;

                        failRetryCount++;

                        var retryInterval = services.RetryStrategy.GetRetryTimeout(failRetryCount);

                        if (retryInterval.TotalSeconds > 0)
                        {
                            await Task.Delay(retryInterval);
                        }

                        continue;
                    }
                }

                var contentTypeValue = httpResponseMessage.Content.Headers
                    .TryGetValues("Content-Type", out var values) ?
                        values.FirstOrDefault() :
                        null;

                var responseContent = new StreamedContent(await httpResponseMessage.Content.ReadAsStreamAsync());

                return await ApplyAfterResponseHandlers(
                    services.AfterResponseHandlers,
                    request,
                    new Response(
                        statusCode: httpResponseMessage.StatusCode,
                        headers: httpResponseMessage.Headers.ToDictionary(
                            kv => kv.Key,
                            kv => kv.Value.First()),
                        content: responseContent,
                        contentType: contentTypeValue,
                        contentEncoding: null,
                        isSuccess: 
                            (int)httpResponseMessage.StatusCode >= 200 &&
                            (int)httpResponseMessage.StatusCode < 300,
                        errorMessage: null,
                        exception: null,
                        request: request,
                        requestedUrl: requestUrl));
            }
        }

        static async Task<(Request Request, string RequestUrl)> ApplyBeforeRequestHandlers(
            IReadOnlyList<RequestServices.BeforeRequestDelegate> handlers,
            Request request,
            string requestUrl)
        {
            var count = handlers.Count;

            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    (request, requestUrl) = await handlers[i](request, requestUrl);
                }
            }

            return (request, requestUrl);
        }

        static async Task<(Request Request, string RequestUrl, bool shouldRetry)> ApplyBeforeRetryHandlers(
            IReadOnlyList<RequestServices.BeforeRetryDelegate> handlers,
            Request request,
            string requestUrl,
            HttpStatusCode statusCode,
            int retryCount)
        {
            var count = handlers.Count;

            var shouldRetry = true;

            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    (request, requestUrl, shouldRetry) = await handlers[i](
                        request, 
                        requestUrl, 
                        statusCode, 
                        retryCount);

                    if (!shouldRetry)
                    {
                        return (request, requestUrl, shouldRetry);
                    }
                }
            }

            return (request, requestUrl, shouldRetry);
        }

        static async Task<Response> ApplyAfterResponseHandlers(
            IReadOnlyList<RequestServices.AfterResponseDelegate> handlers,
            Request request,
            Response response)
        {
            var count = handlers.Count;

            if (count > 0)
            {
                for(var i = 0; i < count; i++)
                {
                    response = await handlers[i](response);
                }
            }

            if(!request.AllowedStatusCodes.Contains(response.StatusCode))
            {
                throw new ApiRequestException(
                    $"Unexpected status code '{response.StatusCode}'",
                    request,
                    response);
            }

            return response;
        }
    }
}
