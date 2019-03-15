using System;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public static partial class RequestExtensions
    {
        public static Request BasicAuthentication(
            this Request request,
            string username,
            string password)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authenticationStrategy: new BasicAuthenticationStrategy(username, password),
                retryStrategy: services.RetryStrategy,
                requestSerializer: services.RequestSerializer,
                responseDeserializer: services.ResponseDeserializer,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
        }

        public static Request BearerTokenAuthentication(
            this Request request,
            Func<Task<IAuthenticationToken>> getBearerToken)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authenticationStrategy: new BearerTokenAuthenticationStrategy(getBearerToken),
                retryStrategy: services.RetryStrategy,
                requestSerializer: services.RequestSerializer,
                responseDeserializer: services.ResponseDeserializer,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
        }

        public static Request CookieAuthentication(
            this Request request,
            Func<Task<IAuthenticationToken>> getCookie)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authenticationStrategy: new CookieAuthenticationStrategy(getCookie),
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
                authenticationStrategy: services.AuthenticationStrategy,
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
                authenticationStrategy: services.AuthenticationStrategy,
                retryStrategy: services.RetryStrategy,
                requestSerializer: services.RequestSerializer,
                responseDeserializer: services.ResponseDeserializer,
                beforeRequestHandlers: services.BeforeRequestHandlers
                    .With(new RequestServices.BeforeRequestDelegate(handler)),
                afterResponseHandlers: services.AfterResponseHandlers));
        }

        public static Request AfterResponse(
            this Request request,
            Func<Request, Response, Task<Response>> handler)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authenticationStrategy: services.AuthenticationStrategy,
                retryStrategy: services.RetryStrategy,
                requestSerializer: services.RequestSerializer,
                responseDeserializer: services.ResponseDeserializer,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                afterResponseHandlers: services.AfterResponseHandlers
                    .With(new RequestServices.AfterResponseDelegate(handler))));
        }

        public static Request JsonRequestSerialization(
            this Request request)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authenticationStrategy: services.AuthenticationStrategy,
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
                authenticationStrategy: services.AuthenticationStrategy,
                retryStrategy: services.RetryStrategy,
                requestSerializer: services.RequestSerializer,
                responseDeserializer: JsonDeserializer.Instance,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
        }
    }
}
