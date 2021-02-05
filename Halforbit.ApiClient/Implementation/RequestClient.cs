using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public async Task<Response> ExecuteAsync(
            Request request,
            CancellationToken cancellationToken = default)
        {
            var services = request.Services;
            var retryContext = new RetryContext(1, services.RetryStrategy?.RetryCount ?? 0);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var response = default(Response);
                var requestUrl = default(string);
                var httpRequestMessage = default(HttpRequestMessage);

                (request, requestUrl, httpRequestMessage) = await BeforeRequestAsync(request);

                cancellationToken.ThrowIfCancellationRequested();

                var requestResult = await ExecuteRequest(GetHttpClient(request), httpRequestMessage, request.Timeout, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                (request, requestUrl, response) = await AfterRequestAsync(request, requestUrl, requestResult, retryContext);

                if (response != null)
                {
                    return response;
                }

                var retryInterval = services.RetryStrategy?.GetRetryTimeout(retryContext.FailureRetryCount) ?? TimeSpan.Zero;
                if (retryInterval.TotalMilliseconds > 50)
                {
                    await Task.Delay(retryInterval, cancellationToken);
                }
            }
        }

        static async Task<(Request Request, string RequestUrl, HttpRequestMessage HttpRequestMessage)> BeforeRequestAsync(
            Request request)
        {
            var services = request.Services;

            if (services.AuthorizationStrategy != null)
            {
                request = await services.AuthorizationStrategy.Apply(request);
            }

            var requestUrl = !string.IsNullOrWhiteSpace(request.BaseUrl)
                ? $"{request.BaseUrl}/{request.Resource}"
                : request.Resource;

            foreach (var kv in request.RouteValues)
            {
                requestUrl = requestUrl.Replace($"{{{kv.Key}}}", kv.Value);
            }

            if (request.QueryValues.Count > 0)
            {
                var query = HttpUtility.ParseQueryString(string.Empty);

                foreach (var kv in request.QueryValues)
                {
                    query[kv.Key] = kv.Value;
                }

                requestUrl = $"{requestUrl}?{query}";
            }

            (request, requestUrl) = await ApplyBeforeRequestHandlers(services.BeforeRequestHandlers,
                request,
                requestUrl);

            var httpRequestMessage = new HttpRequestMessage(method: new HttpMethod(request.Method),
                requestUri: requestUrl);

            if (request.Headers.Count > 0)
            {
                foreach (var kv in request.Headers)
                {
                    httpRequestMessage.Headers.Add(kv.Key, kv.Value);
                }
            }

            if (request.Content != null)
            {
                var requestContent = new StreamContent(request.Content.GetStream());

                if (!string.IsNullOrWhiteSpace(request.ContentType?.Value))
                {
                    requestContent.Headers.Add("Content-Type", request.ContentType.Value);
                }

                httpRequestMessage.Content = requestContent;
            }
            
            return (request, requestUrl, httpRequestMessage);
        }

        static async Task<InternalResponse> ExecuteRequest(
            HttpClient client,
            HttpRequestMessage httpRequestMessage,
            TimeSpan perRequestTimeout,
            CancellationToken externalCancellationToken)
        {
            var internalCts = new CancellationTokenSource();
            var linkedCts = default(CancellationTokenSource);

            if (perRequestTimeout != TimeSpan.Zero && perRequestTimeout != Timeout.InfiniteTimeSpan)
            {
                internalCts.CancelAfter(perRequestTimeout);
            }

            if (externalCancellationToken != CancellationToken.None)
            {
                linkedCts = CancellationTokenSource.CreateLinkedTokenSource(externalCancellationToken, internalCts.Token);
            }

            try
            {
                var token = linkedCts?.Token ?? internalCts.Token;
                var result = await client.SendAsync(httpRequestMessage,
                    HttpCompletionOption.ResponseHeadersRead,
                    token);

                return new InternalResponse()
                {
                    ResponseMessage = result
                };
            }
            catch (OperationCanceledException canceledException)
            {
                return new InternalResponse()
                {
                    RequestTimeout = !externalCancellationToken.IsCancellationRequested,
                    Exception = canceledException
                };
            }
            catch (Exception exception)
            {
                return new InternalResponse()
                {
                    Exception = exception
                };
            }
            finally
            {
                internalCts.Dispose();
                linkedCts?.Dispose();
            }
        }

        static async Task<(Request Request, string RequestUrl, Response Response)> AfterRequestAsync(
            Request request,
            string requestUrl,
            InternalResponse internalResponse,
            RetryContext retryContext)
        {
            var services = request.Services;
            var responseMessage = internalResponse.ResponseMessage;
            var contentTypeValue = default(string);
            var responseContent = default(IContent);
            var responseHeaders = default(IReadOnlyDictionary<string, string>);
            var statusCode = internalResponse.ResponseMessage?.StatusCode ?? default(HttpStatusCode);
            var success = (int) statusCode >= 200 && (int) statusCode < 300;
            var exception = internalResponse.Exception;
            var errorMessage = default(string);

            if (internalResponse.RequestTimeout)
            {
                success = false;
                errorMessage = "Timed out while making request";

                // The previous code would not show the exception on timeouts.
                exception = null; 
                
                var timeoutRetryEnabled = (services.RetryStrategy?.ShouldRetryOnTimeout ?? false);
                if (timeoutRetryEnabled && retryContext.RetryFailure())
                {
                    bool shouldRetry;
                    (request, requestUrl, shouldRetry) = await ApplyBeforeRetryHandlers(request.Services.BeforeRetryHandlers,
                        request,
                        requestUrl,
                        0,
                        retryContext.FailureRetryCount);

                    if (shouldRetry)
                    {
                        return (request, requestUrl, null);
                    }
                }
            }
            else if (exception != null)
            {
                success = false;
                errorMessage = exception.Message;
            }

            if ((services.AuthorizationStrategy?.ShouldReauthorize(statusCode) ?? false) &&
                retryContext.RetryAuthFailure())
            {
                await services.AuthorizationStrategy.Reauthorize();
                return (request, requestUrl, null);
            }

            if ((services.RetryStrategy?.ShouldRetry(statusCode) ?? false) &&
                retryContext.RetryFailure())
            {
                bool shouldRetry;
                (request, requestUrl, shouldRetry) = await ApplyBeforeRetryHandlers(request.Services.BeforeRetryHandlers,
                    request,
                    requestUrl,
                    statusCode,
                    retryContext.FailureRetryCount);

                if (shouldRetry)
                {
                    return (request, requestUrl, null);
                }
            }

            if (responseMessage != null)
            {
                responseHeaders = responseMessage.Headers.ToDictionary(kv => kv.Key,
                    kv => kv.Value.FirstOrDefault());

                contentTypeValue = responseMessage.Content.Headers
                    .TryGetValues("Content-Type", out var values) ?
                    values.FirstOrDefault() :
                    null;
                
                responseContent = new StreamedContent(await responseMessage.Content.ReadAsStreamAsync());
            }

            var response = await ApplyAfterResponseHandlers(services.AfterResponseHandlers,
                request,
                new Response(statusCode: statusCode,
                    headers: responseHeaders,
                    content: responseContent,
                    contentType: contentTypeValue,
                    // This was previously always null, should we deprecate this property?
                    contentEncoding: default,
                    isSuccess: success,
                    errorMessage: errorMessage,
                    exception: exception,
                    request: request,
                    requestedUrl: requestUrl));

            return (request, requestUrl, response);
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
        
        protected virtual HttpClient GetHttpClient(Request request) => _httpClient;
        
        private struct InternalResponse
        {
            public bool RequestTimeout { get; set; }
            public Exception Exception { get; set; }
            public HttpResponseMessage ResponseMessage { get; set; }
        }
        
        private class RetryContext
        {
            private readonly int _authorizationRetries;
            private readonly int _failureRetries;
            
            public RetryContext(
                int authorizationRetries,
                int failureRetries)
            {
                FailureRetryCount = 0;
                AuthRetryCount = 0;
                _authorizationRetries = authorizationRetries;
                _failureRetries = failureRetries;
            }

            public int FailureRetryCount { get; private set; }

            public int AuthRetryCount { get; private set; }

            public bool RetryFailure() => _failureRetries - FailureRetryCount++ > 0;
            public bool RetryAuthFailure() => _authorizationRetries - AuthRetryCount++ > 0;
        }
    }
}
