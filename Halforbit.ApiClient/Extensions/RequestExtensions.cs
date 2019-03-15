using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public static class RequestExtensions
    {
        static readonly Encoding _utf8Encoding = new UTF8Encoding(false);

        // Strategy ///////////////////////////////////////////////////////////

        public static Request BasicAuthentication(
            this Request request,
            string username,
            string password)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return new Request(
                services: new RequestServices(
                    requestClient: services.RequestClient,
                    authenticationStrategy: new BasicAuthenticationStrategy(username, password),
                    retryStrategy: services.RetryStrategy,
                    requestSerializer: services.RequestSerializer,
                    responseDeserializer: services.ResponseDeserializer,
                    beforeRequestHandlers: services.BeforeRequestHandlers,
                    afterResponseHandlers: services.AfterResponseHandlers),
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request BearerTokenAuthentication(
            this Request request,
            Func<Task<IAuthenticationToken>> getBearerToken)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return new Request(
                services: new RequestServices(
                    requestClient: services.RequestClient,
                    authenticationStrategy: new BearerTokenAuthenticationStrategy(getBearerToken),
                    retryStrategy: services.RetryStrategy,
                    requestSerializer: services.RequestSerializer,
                    responseDeserializer: services.ResponseDeserializer,
                    beforeRequestHandlers: services.BeforeRequestHandlers,
                    afterResponseHandlers: services.AfterResponseHandlers),
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request CookieAuthentication(
            this Request request,
            Func<Task<IAuthenticationToken>> getCookie)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return new Request(
                services: new RequestServices(
                    requestClient: services.RequestClient,
                    authenticationStrategy: new CookieAuthenticationStrategy(getCookie),
                    retryStrategy: services.RetryStrategy,
                    requestSerializer: services.RequestSerializer,
                    responseDeserializer: services.ResponseDeserializer,
                    beforeRequestHandlers: services.BeforeRequestHandlers,
                    afterResponseHandlers: services.AfterResponseHandlers),
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request Retry(
            this Request request,
            int retryCount = 5,
            bool retryOnTimeout = false)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return new Request(
                services: new RequestServices(
                    requestClient: services.RequestClient,
                    authenticationStrategy: services.AuthenticationStrategy,
                    retryStrategy: new ExponentialBackoffRetryStrategy(
                        retryCount,
                        retryOnTimeout),
                    requestSerializer: services.RequestSerializer,
                    responseDeserializer: services.ResponseDeserializer,
                    beforeRequestHandlers: services.BeforeRequestHandlers,
                    afterResponseHandlers: services.AfterResponseHandlers),
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request BeforeRequest(
            this Request request,
            Func<Request, string, Task<(Request, string)>> handler)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return new Request(
                services: new RequestServices(
                    requestClient: services.RequestClient,
                    authenticationStrategy: services.AuthenticationStrategy,
                    retryStrategy: services.RetryStrategy,
                    requestSerializer: services.RequestSerializer,
                    responseDeserializer: services.ResponseDeserializer,
                    beforeRequestHandlers: services.BeforeRequestHandlers
                        .With(new RequestServices.BeforeRequestDelegate(handler)),
                    afterResponseHandlers: services.AfterResponseHandlers),
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request AfterResponse(
            this Request request,
            Func<Request, Response, Task<Response>> handler)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return new Request(
                services: new RequestServices(
                    requestClient: services.RequestClient,
                    authenticationStrategy: services.AuthenticationStrategy,
                    retryStrategy: services.RetryStrategy,
                    requestSerializer: services.RequestSerializer,
                    responseDeserializer: services.ResponseDeserializer,
                    beforeRequestHandlers: services.BeforeRequestHandlers,
                    afterResponseHandlers: services.AfterResponseHandlers
                        .With(new RequestServices.AfterResponseDelegate(handler))),
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request JsonRequestSerialization(
            this Request request)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return new Request(
                services: new RequestServices(
                    requestClient: services.RequestClient,
                    authenticationStrategy: services.AuthenticationStrategy,
                    retryStrategy: services.RetryStrategy,
                    requestSerializer: JsonSerializer.Instance,
                    responseDeserializer: services.ResponseDeserializer,
                    beforeRequestHandlers: services.BeforeRequestHandlers,
                    afterResponseHandlers: services.AfterResponseHandlers),
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request JsonResponseSerialization(
            this Request request)
        {
            request = request ?? Request.Default;

            var services = request.Services;

            return new Request(
                services: new RequestServices(
                    requestClient: services.RequestClient,
                    authenticationStrategy: services.AuthenticationStrategy,
                    retryStrategy: services.RetryStrategy,
                    requestSerializer: services.RequestSerializer,
                    responseDeserializer: JsonDeserializer.Instance,
                    beforeRequestHandlers: services.BeforeRequestHandlers,
                    afterResponseHandlers: services.AfterResponseHandlers),
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        // Request ////////////////////////////////////////////////////////////

        public static Request BaseUrl(
            this Request request,
            string baseUrl)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: baseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request Method(
            this Request request,
            string method)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request Resource(
            this Request request,
            string resource)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request RouteValue(
            this Request request,
            string key,
            string value)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues.With(key, value),
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request RouteValues(
            this Request request,
            object values)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues.With(values),
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request RouteValues(
            this Request request,
            params (string Key, string Value)[] values)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues.With(values),
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request RouteValues(
            this Request request,
            IReadOnlyDictionary<string, string> values)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues.With(values),
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request QueryValue(
            this Request request,
            string key,
            string value)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues.With(key, value),
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request QueryValues(
            this Request request,
            object values)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues.With(values),
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request QueryValues(
            this Request request,
            params (string Key, string Value)[] values)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues.With(values),
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request QueryValues(
            this Request request,
            IReadOnlyDictionary<string, string> values)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues.With(values),
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request Header(
            this Request request,
            string key,
            string value)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers.With(key, value),
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request Headers(
            this Request request,
            object values)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers.With(values),
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request Headers(
            this Request request,
            params (string Key, string Value)[] values)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers.With(values),
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request Headers(
            this Request request,
            IReadOnlyDictionary<string, string> values)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers.With(values),
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request ContentType(
            this Request request,
            string contentType)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: contentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request ContentEncoding(
            this Request request,
            string contentEncoding)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: contentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request Authentication(
            this Request request,
            string authentication)
        {
            request = request ?? Request.Default;

            return request.Header("Authentication", authentication);
        }

        public static Request Cookie(
            this Request request,
            string cookie)
        {
            request = request ?? Request.Default;

            return request.Header("Cookie", cookie);
        }

        public static Request Timeout(
            this Request request,
            TimeSpan timeout)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request Allow(
            this Request request,
            params HttpStatusCode[] statusCodes)
        {
            request = request ?? Request.Default;

            var expectedStatusCodes = new HashSet<HttpStatusCode>(request.AllowedStatusCodes);

            foreach(var statusCode in statusCodes)
            {
                expectedStatusCodes.Add(statusCode);
            }

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: expectedStatusCodes,
                tags: request.Tags);
        }

        public static Request AllowNotFound(
            this Request request)
        {
            request = request ?? Request.Default;

            return request.Allow(HttpStatusCode.NotFound);
        }

        public static Request AllowAnyStatusCode(
            this Request request)
        {
            request = request ?? Request.Default;

            return request.Allow(new HttpStatusCode[] { 0 }.Concat(Enum
                .GetValues(typeof(HttpStatusCode))
                .Cast<HttpStatusCode>())
                .ToArray());
        }

        public static Request Tag<TValue>(
            this Request request,
            string key, 
            TValue value)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags.With(key, value));
        }

        public static Request Tag(
            this Request request,
            IReadOnlyDictionary<string, object> tags)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags.With(tags));
        }

        public static Request Tag<TValue>(
            this Request request,
            params (string Key, object Value)[] tags)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags.With(tags));
        }

        public static TValue Tag<TValue>(
            this Request request, 
            string key)
        {
            return request.Tags.TryGetValue(key, out var value) ? 
                (TValue)value : 
                default;
        }

        public static Request Name(
            this Request request,
            string name)
        {
            request = request ?? Request.Default;

            return request.Tag(nameof(Name), name);
        }

        public static string Name(this Request request)
        {
            return request.Tag<string>(nameof(Name));
        }

        // Request Body ///////////////////////////////////////////////////////

        public static Request TextBody(
            this Request request,
            string body,
            string mediaType = "text/plain")
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: new BufferedContent(_utf8Encoding.GetBytes(body)),
                contentType: $"{mediaType}; charset=utf-8",
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request Body(
            this Request request,
            Stream stream,
            string contentType = default)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: new StreamedContent(stream),
                contentType: contentType ?? request.Services.RequestSerializer.ContentType ?? "application/octet-stream",
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request Body<TBody>(
            this Request request,
            TBody body)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: new ObjectContent<TBody>(body, request.Services.RequestSerializer),
                contentType: request.Services.RequestSerializer.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request FormBody(
            this Request request,
            IReadOnlyDictionary<string, string> formValues)
        {
            request = request ?? Request.Default;

            return new Request(
                services: request.Services,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: new BufferedContent(_utf8Encoding.GetBytes(
                    new FormUrlEncodedContent(formValues).ReadAsStringAsync().Result)),
                contentType: $"application/x-www-form-urlencoded; charset=utf-8",
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request FormBody(
            this Request request,
            params (string Key, string Value)[] formValues)
        {
            request = request ?? Request.Default;

            return request.FormBody(formValues.ToReadOnlyDictionary());
        }

        // Execution //////////////////////////////////////////////////////////

        public static async Task<Response> ExecuteAsync(this Request request)
        {
            return await request.Services.RequestClient.ExecuteAsync(request);
        }

        public static async Task<Response> GetAsync(
            this Request request,
            string resource = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("GET")
                .ExecuteAsync();
        }

        public static async Task<Response> PostAsync(
            this Request request,
            string resource = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("POST")
                .ExecuteAsync();
        }

        public static async Task<Response> PutAsync(
            this Request request,
            string resource = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("PUT")
                .ExecuteAsync();
        }

        public static async Task<Response> PatchAsync(
            this Request request,
            string resource = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("PATCH")
                .ExecuteAsync();
        }

        public static async Task<Response> DeleteAsync(
            this Request request,
            string resource = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("DELETE")
                .ExecuteAsync();
        }
    }
}
