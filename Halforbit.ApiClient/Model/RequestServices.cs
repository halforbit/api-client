using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public class RequestServices : IDisposable
    {
        public RequestServices(
            IRequestClient requestClient,
            IAuthorizationStrategy authorizationStrategy,
            IRetryStrategy retryStrategy,
            ISerializer requestSerializer,
            IDeserializer responseDeserializer,
            IReadOnlyList<BeforeRequestDelegate> beforeRequestHandlers,
            IReadOnlyList<AfterResponseDelegate> afterResponseHandlers,
            IReadOnlyList<BeforeRetryDelegate> beforeRetryHandlers)
        {
            RequestClient = requestClient;

            AuthorizationStrategy = authorizationStrategy;

            RetryStrategy = retryStrategy;

            RequestSerializer = requestSerializer;

            ResponseDeserializer = responseDeserializer;

            BeforeRequestHandlers = beforeRequestHandlers;

            AfterResponseHandlers = afterResponseHandlers;

            BeforeRetryHandlers = beforeRetryHandlers;
        }

        public IRequestClient RequestClient { get; }

        public IAuthorizationStrategy AuthorizationStrategy { get; }

        public IRetryStrategy RetryStrategy { get; }

        public ISerializer RequestSerializer { get; }

        public IDeserializer ResponseDeserializer { get; }

        public IReadOnlyList<BeforeRequestDelegate> BeforeRequestHandlers { get; }

        public IReadOnlyList<AfterResponseDelegate> AfterResponseHandlers { get; }

        public IReadOnlyList<BeforeRetryDelegate> BeforeRetryHandlers { get; }

        public static RequestServices Default => new RequestServices(
            requestClient: Halforbit.ApiClient.RequestClient.Instance, 
            authorizationStrategy: default,
            retryStrategy: default,
            requestSerializer: JsonSerializer.Instance,
            responseDeserializer: JsonDeserializer.Instance,
            beforeRequestHandlers: new List<BeforeRequestDelegate>(0),
            afterResponseHandlers: new List<AfterResponseDelegate>(0),
            beforeRetryHandlers: new List<BeforeRetryDelegate>(0));

        public delegate Task<(Request request, string requestUrl)> BeforeRequestDelegate(
            Request request,
            string requestUrl);

        public delegate Task<Response> AfterResponseDelegate(Response response);

        public delegate Task<(Request Request, string RequestUrl, bool ShouldRetry)> BeforeRetryDelegate(
            Request request,
            string requestUrl,
            HttpStatusCode statusCode,
            int retryCount);

        public void Dispose()
        {
            RequestClient.Dispose();
        }
    }
}
