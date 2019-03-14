using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Response> Execute(Request request)
        {
            var reauthorizeRetriesRemaining = 1;

            var failRetriesRemaining = request.RetryStrategy?.RetryCount ?? 0;

            var failRetryCount = 0;

            while(true)
            {
                if(request.AuthenticationStrategy != null)
                {
                    request = await request.AuthenticationStrategy.Apply(request);
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

                var (newRequest, newRequestUrl) = await ApplyBeforeRequestHandlers(
                    request.BeforeRequestHandlers, 
                    request, 
                    requestUrl);

                request = newRequest;

                requestUrl = newRequestUrl;

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

                    requestContent.Headers.Add("Content-Type", request.ContentType);

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
                        if (request.RetryStrategy?.ShouldRetryOnTimeout ?? false && 
                            failRetriesRemaining > 0)
                        {
                            failRetriesRemaining--;

                            failRetryCount++;

                            var retryInterval = request.RetryStrategy.GetRetryTimeout(failRetryCount);

                            if (retryInterval.TotalSeconds > 0)
                            {
                                await Task.Delay(retryInterval);
                            }

                            continue;
                        }
                        else
                        {
                            return await ApplyAfterResponseHandlers(
                                request.AfterResponseHandlers, 
                                request, 
                                requestUrl,
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
                        request.AfterResponseHandlers,
                        request,
                        requestUrl,
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

                var s = await httpResponseMessage.Content.ReadAsStreamAsync();

                if (request.AuthenticationStrategy?.ShouldReauthenticate(httpResponseMessage.StatusCode) ?? false && 
                    reauthorizeRetriesRemaining > 0)
                {
                    await request.AuthenticationStrategy.Reauthenticate();

                    reauthorizeRetriesRemaining--;

                    continue;
                }

                if (request.RetryStrategy?.ShouldRetry(httpResponseMessage.StatusCode) ?? false && 
                    failRetriesRemaining > 0)
                { 
                    failRetriesRemaining--;

                    failRetryCount++;

                    var retryInterval = request.RetryStrategy.GetRetryTimeout(failRetryCount);

                    if(retryInterval.TotalSeconds > 0)
                    {
                        await Task.Delay(retryInterval);
                    }

                    continue;
                }

                var contentTypeValue = httpResponseMessage.Content.Headers
                    .TryGetValues("Content-Type", out var values) ?
                        values.FirstOrDefault() :
                        null;

                var contentType = !string.IsNullOrWhiteSpace(contentTypeValue) ?
                    new System.Net.Mime.ContentType(contentTypeValue) :
                    null;

                var responseContent = new StreamedContent(await httpResponseMessage.Content.ReadAsStreamAsync());

                return await ApplyAfterResponseHandlers(
                    request.AfterResponseHandlers,
                    request,
                    requestUrl,
                    new Response(
                        statusCode: httpResponseMessage.StatusCode,
                        headers: httpResponseMessage.Headers.ToDictionary(
                            kv => kv.Key,
                            kv => kv.Value.First()),
                        content: responseContent,
                        contentType: contentType == null ? null : new ContentType(
                            boundary: contentType.Boundary,
                            charSet: contentType.CharSet,
                            mediaType: contentType.MediaType,
                            name: contentType.Name),
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
            IReadOnlyList<Request.BeforeRequestDelegate> handlers,
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

        static async Task<Response> ApplyAfterResponseHandlers(
            IReadOnlyList<Request.AfterResponseDelegate> handlers,
            Request request,
            string requestUrl,
            Response response)
        {
            var count = handlers.Count;

            if (count > 0)
            {
                for(var i = 0; i < count; i++)
                {
                    response = await handlers[i](request, requestUrl, response);
                }
            }

            return response;
        }
    }
}
