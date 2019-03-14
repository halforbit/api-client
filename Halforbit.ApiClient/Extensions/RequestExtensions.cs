using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: new BasicAuthenticationStrategy(username, password),
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request BearerTokenAuthentication(
            this Request request,
            Func<Task<IAuthenticationToken>> getBearerToken)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: new BearerTokenAuthenticationStrategy(getBearerToken),
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request CookieAuthentication(
            this Request request,
            Func<Task<IAuthenticationToken>> getCookie)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: new CookieAuthenticationStrategy(getCookie),
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request Retry(
            this Request request,
            int retryCount = 5,
            bool retryOnTimeout = false)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: new ExponentialBackoffRetryStrategy(
                    retryCount,
                    retryOnTimeout),
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request BeforeRequest(
            this Request request,
            Func<Request, string, Task<(Request, string)>> handler)
        {
            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers
                    .With(new Request.BeforeRequestDelegate(handler)),
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request AfterResponse(
            this Request request,
            Func<Request, string, Response, Task<Response>> handler)
        {
            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers
                .With(new Request.AfterResponseDelegate(handler)),
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        // Request ////////////////////////////////////////////////////////////

        public static Request Name(
            this Request request,
            string name)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request BaseUrl(
            this Request request,
            string baseUrl)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: baseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request Method(
            this Request request,
            string method)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request Resource(
            this Request request,
            string resource)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request RouteValue(
            this Request request,
            string key,
            string value)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues.With(key, value),
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request RouteValues(
            this Request request,
            object values)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues.With(values),
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request RouteValues(
            this Request request,
            params (string Key, string Value)[] values)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues.With(values),
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request RouteValues(
            this Request request,
            IReadOnlyDictionary<string, string> values)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues.With(values),
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request QueryValue(
            this Request request,
            string key,
            string value)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues.With(key, value),
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request QueryValues(
            this Request request,
            object values)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues.With(values),
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request QueryValues(
            this Request request,
            params (string Key, string Value)[] values)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues.With(values),
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request QueryValues(
            this Request request,
            IReadOnlyDictionary<string, string> values)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues.With(values),
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request Header(
            this Request request,
            string key,
            string value)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers.With(key, value),
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request Headers(
            this Request request,
            object values)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers.With(values),
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request Headers(
            this Request request,
            params (string Key, string Value)[] values)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers.With(values),
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request Headers(
            this Request request,
            IReadOnlyDictionary<string, string> values)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers.With(values),
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request ContentType(
            this Request request,
            string contentType)
        {
            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: contentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request ContentEncoding(
            this Request request,
            string contentEncoding)
        {
            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: contentEncoding,
                timeout: request.Timeout);
        }

        public static Request Authentication(
            this Request request,
            string authentication)
        {
            return request.Header("Authentication", authentication);
        }

        public static Request Cookie(
            this Request request,
            string cookie)
        {
            return request.Header("Cookie", cookie);
        }

        public static Request Timeout(
            this Request request,
            TimeSpan timeout)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: request.Content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: timeout);
        }

        // Request Body ///////////////////////////////////////////////////////

        public static Request TextBody(
            this Request request,
            string body,
            string mediaType = "text/plain")
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                name: request.Name,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: new BufferedContent(_utf8Encoding.GetBytes(body)),
                contentType: $"{mediaType}; charset=utf-8",
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request JsonBody(
            this Request request,
            object body)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
                baseUrl: request.BaseUrl,
                method: request.Method,
                resource: request.Resource,
                headers: request.Headers,
                routeValues: request.RouteValues,
                queryValues: request.QueryValues,
                content: new BufferedContent(_utf8Encoding.GetBytes(JsonConvert.SerializeObject(body))),
                contentType: $"application/json; charset=utf-8",
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout);
        }

        public static Request FormBody(
            this Request request,
            IReadOnlyDictionary<string, string> formValues)
        {
            request = request ?? Request.Default;

            return new Request(
                requestClient: request.RequestClient,
                authenticationStrategy: request.AuthenticationStrategy,
                retryStrategy: request.RetryStrategy,
                beforeRequestHandlers: request.BeforeRequestHandlers,
                afterResponseHandlers: request.AfterResponseHandlers,
                name: request.Name,
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
                timeout: request.Timeout);
        }

        public static Request FormBody(
            this Request request,
            params (string Key, string Value)[] formValues)
        {
            return request.FormBody(formValues.ToReadOnlyDictionary());
        }

        // Execution //////////////////////////////////////////////////////////

        public static async Task<Response> Execute(this Request request)
        {
            return await request.RequestClient.Execute(request);
        }

        public static async Task<Response> Get(
            this Request request,
            string resource)
        {
            return await request.Method("GET").Resource(resource).Execute();
        }

        public static async Task<Response> Post(
            this Request request,
            string resource)
        {
            return await request.Method("POST").Resource(resource).Execute();
        }

        public static async Task<Response> Put(
            this Request request,
            string resource)
        {
            return await request.Method("PUT").Resource(resource).Execute();
        }

        public static async Task<Response> Patch(
            this Request request,
            string resource)
        {
            return await request.Method("PATCH").Resource(resource).Execute();
        }

        public static async Task<Response> Delete(
            this Request request,
            string resource)
        {
            return await request.Method("DELETE").Resource(resource).Execute();
        }
    }
}
