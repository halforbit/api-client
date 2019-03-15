using System.Collections.Generic;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public class RequestServices
    {
        public RequestServices(
            IRequestClient requestClient,
            IAuthenticationStrategy authenticationStrategy,
            IRetryStrategy retryStrategy,
            ISerializer requestSerializer,
            IDeserializer responseDeserializer,
            IReadOnlyList<BeforeRequestDelegate> beforeRequestHandlers,
            IReadOnlyList<AfterResponseDelegate> afterResponseHandlers)
        {
            RequestClient = requestClient;

            AuthenticationStrategy = authenticationStrategy;

            RetryStrategy = retryStrategy;

            RequestSerializer = requestSerializer;

            ResponseDeserializer = responseDeserializer;

            BeforeRequestHandlers = beforeRequestHandlers;

            AfterResponseHandlers = afterResponseHandlers;
        }

        public IRequestClient RequestClient { get; }

        public IAuthenticationStrategy AuthenticationStrategy { get; }

        public IRetryStrategy RetryStrategy { get; }

        public ISerializer RequestSerializer { get; }

        public IDeserializer ResponseDeserializer { get; }

        public IReadOnlyList<BeforeRequestDelegate> BeforeRequestHandlers { get; }

        public IReadOnlyList<AfterResponseDelegate> AfterResponseHandlers { get; }

        public static RequestServices Default => new RequestServices(
            requestClient: Halforbit.ApiClient.RequestClient.Instance,
            authenticationStrategy: default,
            retryStrategy: default,
            requestSerializer: JsonSerializer.Instance,
            responseDeserializer: JsonDeserializer.Instance,
            beforeRequestHandlers: new List<BeforeRequestDelegate>(0),
            afterResponseHandlers: new List<AfterResponseDelegate>(0));

        public delegate Task<(Request request, string requestUrl)> BeforeRequestDelegate(
            Request request,
            string requestUrl);

        public delegate Task<Response> AfterResponseDelegate(
            Request request,
            string requestUrl,
            Response response);
    }
}
