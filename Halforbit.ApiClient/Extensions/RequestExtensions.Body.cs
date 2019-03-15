using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Halforbit.ApiClient
{
    public static partial class RequestExtensions
    {
        static readonly Encoding _utf8Encoding = new UTF8Encoding(false);

        public static Request Body(
            this Request request,
			byte[] body, 
			string mediaType = default)
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
                content: new BufferedContent(body),
                contentType: !string.IsNullOrWhiteSpace(mediaType) ? 
					$"{mediaType}; charset=utf-8" : 
					request.ContentType,
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
    }
}
