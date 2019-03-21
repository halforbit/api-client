using System;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public static partial class RequestExtensions
    {
        public static Request RequestClient(
            this Request request,
            IRequestClient requestClient)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: requestClient,
                authorizationStrategy: services.AuthorizationStrategy,
                retryStrategy: services.RetryStrategy,
                requestSerializer: services.RequestSerializer,
                responseDeserializer: services.ResponseDeserializer,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
        }

        public static Request BasicAuthorization(
            this Request request,
            string username,
            string password)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authorizationStrategy: new BasicAuthorizationStrategy(username, password),
                retryStrategy: services.RetryStrategy,
                requestSerializer: services.RequestSerializer,
                responseDeserializer: services.ResponseDeserializer,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
        }

        public static Request BearerTokenAuthorization(
            this Request request,
            Func<Task<IAuthorizationToken>> getBearerToken)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authorizationStrategy: new BearerTokenAuthorizationStrategy(getBearerToken),
                retryStrategy: services.RetryStrategy,
                requestSerializer: services.RequestSerializer,
                responseDeserializer: services.ResponseDeserializer,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
        }

        public static Request CookieAuthorization(
            this Request request,
            Func<Task<IAuthorizationToken>> getCookie)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authorizationStrategy: new CookieAuthorizationStrategy(getCookie),
                retryStrategy: services.RetryStrategy,
                requestSerializer: services.RequestSerializer,
                responseDeserializer: services.ResponseDeserializer,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
        }

        public static Request Retry(
            this Request request,
            int retryCount = 5,
            bool retryOnTimeout = false)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authorizationStrategy: services.AuthorizationStrategy,
                retryStrategy: new ExponentialBackoffRetryStrategy(
                    retryCount,
                    retryOnTimeout),
                requestSerializer: services.RequestSerializer,
                responseDeserializer: services.ResponseDeserializer,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
        }

        public static Request BeforeRequest(
            this Request request,
            Func<Request, string, Task<(Request, string)>> handler)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authorizationStrategy: services.AuthorizationStrategy,
                retryStrategy: services.RetryStrategy,
                requestSerializer: services.RequestSerializer,
                responseDeserializer: services.ResponseDeserializer,
                beforeRequestHandlers: services.BeforeRequestHandlers
                    .WithFirst(new RequestServices.BeforeRequestDelegate(handler)),
                afterResponseHandlers: services.AfterResponseHandlers));
        }

        public static Request AfterResponse(
            this Request request,
            Func<Response, Task<Response>> handler)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authorizationStrategy: services.AuthorizationStrategy,
                retryStrategy: services.RetryStrategy,
                requestSerializer: services.RequestSerializer,
                responseDeserializer: services.ResponseDeserializer,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                afterResponseHandlers: services.AfterResponseHandlers
                    .WithFirst(new RequestServices.AfterResponseDelegate(handler))));
        }

        public static Request JsonRequestSerialization(
            this Request request)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authorizationStrategy: services.AuthorizationStrategy,
                retryStrategy: services.RetryStrategy,
                requestSerializer: JsonSerializer.Instance,
                responseDeserializer: services.ResponseDeserializer,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
        }

        public static Request JsonResponseSerialization(
            this Request request)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authorizationStrategy: services.AuthorizationStrategy,
                retryStrategy: services.RetryStrategy,
                requestSerializer: services.RequestSerializer,
                responseDeserializer: JsonDeserializer.Instance,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
        }
    }
}
