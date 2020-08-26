using System;
using System.Collections.Generic;
using System.Net;

namespace Halforbit.ApiClient
{
    public class Request : IDisposable
    {
        static readonly IReadOnlyDictionary<string, string> _emptyDictionary = new Dictionary<string, string>(0);

        public Request(
            RequestServices services,
            string baseUrl,
            string method,
            string resource,
            IReadOnlyDictionary<string, string> headers,
            IReadOnlyDictionary<string, string> routeValues,
            IReadOnlyDictionary<string, string> queryValues,
            IContent content,
            ContentType contentType,
            string contentEncoding,
            TimeSpan timeout,
            IReadOnlyCollection<HttpStatusCode> allowedStatusCodes,
            IReadOnlyCollection<HttpStatusCode> defaultContentStatusCodes,
            IReadOnlyDictionary<string, object> tags)
        {
            Services = services;

            BaseUrl = baseUrl;

            Method = method;

            Resource = resource;

            Headers = headers;

            RouteValues = routeValues;

            QueryValues = queryValues;

            Content = content;

            ContentType = contentType;

            ContentEncoding = contentEncoding;

            Timeout = timeout;

            AllowedStatusCodes = allowedStatusCodes;

            DefaultContentStatusCodes = defaultContentStatusCodes;

            Tags = tags;
        }

        public RequestServices Services { get; }

        public string BaseUrl { get; }

        public string Method { get; }

        public string Resource { get; }

        public IReadOnlyDictionary<string, string> Headers { get; }

        public IReadOnlyDictionary<string, string> RouteValues { get; }

        public IReadOnlyDictionary<string, string> QueryValues { get; }

        public IContent Content { get; }

        public ContentType ContentType { get; }

        public string ContentEncoding { get; }

        public TimeSpan Timeout { get; }

        public IReadOnlyCollection<HttpStatusCode> AllowedStatusCodes { get; }

        public IReadOnlyCollection<HttpStatusCode> DefaultContentStatusCodes { get; }

        public IReadOnlyDictionary<string, object> Tags { get; }

        public static Request Default => new Request(
            services: RequestServices.Default,
            baseUrl: default,
            method: default,
            resource: default,
            headers: _emptyDictionary,
            routeValues: _emptyDictionary,
            queryValues: _emptyDictionary,
            content: default,
            contentType: default,
            contentEncoding: default,
            timeout: TimeSpan.FromSeconds(100),
            allowedStatusCodes: new HashSet<HttpStatusCode>
            {
                HttpStatusCode.OK,
                HttpStatusCode.Created,
                HttpStatusCode.Accepted,
                HttpStatusCode.NonAuthoritativeInformation,
                HttpStatusCode.NoContent,
                HttpStatusCode.ResetContent,
                HttpStatusCode.PartialContent
            },
            defaultContentStatusCodes: new HashSet<HttpStatusCode>
            {
                HttpStatusCode.NoContent
            },
            tags: new Dictionary<string, object>(0)); 

        public static Request Create(
            string baseUrl = default,
            IRequestClient requestClient = default)
        {
            var source = Default;

            return new Request(
                services: source.Services,
                baseUrl: baseUrl ?? source.BaseUrl,
                method: source.Method,
                resource: source.Resource,
                headers: source.Headers,
                routeValues: source.RouteValues,
                queryValues: source.QueryValues,
                content: source.Content,
                contentType: source.ContentType,
                contentEncoding: source.ContentEncoding,
                timeout: source.Timeout,
                allowedStatusCodes: source.AllowedStatusCodes,
                defaultContentStatusCodes: source.DefaultContentStatusCodes,
                tags: source.Tags);
        }

        public void Dispose()
        {
            Services.Dispose();
        }
    }
}
