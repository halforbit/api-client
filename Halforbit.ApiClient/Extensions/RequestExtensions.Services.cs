using Halforbit.ApiClient.Implementation;
using System;
using System.Net;
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
                beforeRetryHandlers: services.BeforeRetryHandlers,
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
                beforeRetryHandlers: services.BeforeRetryHandlers,
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
                beforeRetryHandlers: services.BeforeRetryHandlers,
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
                beforeRetryHandlers: services.BeforeRetryHandlers,
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
                beforeRetryHandlers: services.BeforeRetryHandlers,
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
                beforeRetryHandlers: services.BeforeRetryHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
        }

        public static Request BeforeRetry(
            this Request request,
            Func<Request, string, HttpStatusCode, int, Task<(Request, string, bool)>> handler)
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
                beforeRetryHandlers: services.BeforeRetryHandlers
                    .WithFirst(new RequestServices.BeforeRetryDelegate(handler)),
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
                beforeRetryHandlers: services.BeforeRetryHandlers,
                afterResponseHandlers: services.AfterResponseHandlers
                    .WithFirst(new RequestServices.AfterResponseDelegate(handler))));
        }

        public static Request RequestSerializer(
            this Request request,
            ISerializer serializer)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authorizationStrategy: services.AuthorizationStrategy,
                retryStrategy: services.RetryStrategy,
                requestSerializer: serializer,
                responseDeserializer: services.ResponseDeserializer,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                beforeRetryHandlers: services.BeforeRetryHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
        }

        public static Request ResponseDeserializer(
            this Request request,
            IDeserializer deserializer)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authorizationStrategy: services.AuthorizationStrategy,
                retryStrategy: services.RetryStrategy,
                requestSerializer: services.RequestSerializer,
                responseDeserializer: deserializer,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                beforeRetryHandlers: services.BeforeRetryHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
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
                beforeRetryHandlers: services.BeforeRetryHandlers,
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
                beforeRetryHandlers: services.BeforeRetryHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
        }

        public static Request XmlRequestSerialization(
            this Request request)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authorizationStrategy: services.AuthorizationStrategy,
                retryStrategy: services.RetryStrategy,
                requestSerializer: XmlSerializer.Instance,
                responseDeserializer: services.ResponseDeserializer,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                beforeRetryHandlers: services.BeforeRetryHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
        }

        public static Request XmlResponseSerialization(
            this Request request)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return request.Services(new RequestServices(
                requestClient: services.RequestClient,
                authorizationStrategy: services.AuthorizationStrategy,
                retryStrategy: services.RetryStrategy,
                requestSerializer: services.RequestSerializer,
                responseDeserializer: XmlDeserializer.Instance,
                beforeRequestHandlers: services.BeforeRequestHandlers,
                beforeRetryHandlers: services.BeforeRetryHandlers,
                afterResponseHandlers: services.AfterResponseHandlers));
        }
    }
}
