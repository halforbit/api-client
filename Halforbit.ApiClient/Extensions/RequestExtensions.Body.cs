using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Halforbit.ApiClient
{
    public static partial class RequestExtensions
    {
        static readonly Encoding _utf8Encoding = new UTF8Encoding(false);

        static readonly Encoding _iso_8859_1_Encoding = Encoding.GetEncoding(28591);

        public static Request Body(
            this Request request,
            IContent content)
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
                content: content,
                contentType: request.ContentType,
                contentEncoding: request.ContentEncoding,
                timeout: request.Timeout,
                allowedStatusCodes: request.AllowedStatusCodes,
                tags: request.Tags);
        }

        public static Request Body(
            this Request request,
			byte[] body, 
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
                content: new BufferedContent(body),
                contentType: contentType ?? request.ContentType ?? "application/octet-stream",
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
                contentType: contentType ?? request.ContentType ?? "application/octet-stream",
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

        public static string TextBody(
            this Request request)
        {
            var encoding = _utf8Encoding;

            if (request.ContentType?.Value != null)
            {
                var charSet = (request.ContentType.CharSet ?? string.Empty).ToLower();

                switch (charSet)
                {
                    case "ascii":
                    case "us-ascii":
                        encoding = Encoding.ASCII;
                        break;

                    case "":
                    case "utf-8":
                        // Already set.
                        break;

                    case "iso-8859-1":
                        encoding = _iso_8859_1_Encoding;
                        break;

                    default: throw new Exception($"Unhandled charset '{request.ContentType.CharSet}'");
                }
            }

            using (var sr = new StreamReader(request.Content.GetStream(), encoding))
            {
                return sr.ReadToEnd();
            }
        }

        public static bool BodyIsText(
            this Request request)
        {
            var mediaType = request.ContentType?.MediaType;

            return mediaType.StartsWith("text/") || mediaType == "application/json";
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

        public static (Request Source, Request Peeked) PeekBody(
            this Request request,
            int maxPeekLength = 1_024_000)
        {
            if (request.Content is StreamedContent streamedContent)
            {
                var peekStream = new PeekStream(
                    streamedContent.GetStream(), 
                    maxPeekLength);

                return (
                    request.Body(new StreamedContent(peekStream)),
                    request.Body(new BufferedContent(peekStream.PeekedData)));
            }
            else
            {
                return (request, request);
            }
        }
    }
}
