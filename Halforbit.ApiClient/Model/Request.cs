using System;
using System.Collections.Generic;

namespace Halforbit.ApiClient
{
    public class Request
    {
        static readonly IReadOnlyDictionary<string, string> _emptyDictionary = new Dictionary<string, string>(0);

        public Request(
            RequestServices services,
            string name,
            string baseUrl,
            string method,
            string resource,
            IReadOnlyDictionary<string, string> headers,
            IReadOnlyDictionary<string, string> routeValues,
            IReadOnlyDictionary<string, string> queryValues,
            IContent content,
            string contentType,
            string contentEncoding,
            TimeSpan timeout)
        {
            Services = services;

            Name = name;

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
        }

        public RequestServices Services { get; }

        public string Name { get; }

        public string BaseUrl { get; }

        public string Method { get; }

        public string Resource { get; }

        public IReadOnlyDictionary<string, string> Headers { get; }

        public IReadOnlyDictionary<string, string> RouteValues { get; }

        public IReadOnlyDictionary<string, string> QueryValues { get; }

        public IContent Content { get; }

        public string ContentType { get; }

        public string ContentEncoding { get; }

        public TimeSpan Timeout { get; }

        public static Request Default => new Request(
            services: RequestServices.Default,
            name: default,
            baseUrl: default,
            method: default,
            resource: default,
            headers: _emptyDictionary,
            routeValues: _emptyDictionary,
            queryValues: _emptyDictionary,
            content: default,
            contentType: default,
            contentEncoding: default,
            timeout: TimeSpan.FromSeconds(100));

        public static Request Create(
            string baseUrl = default,
            IRequestClient requestClient = default)
        {
            var source = Default;

            return new Request(
                services: source.Services,
                name: source.Name,
                baseUrl: baseUrl ?? source.BaseUrl,
                method: source.Method,
                resource: source.Resource,
                headers: source.Headers,
                routeValues: source.RouteValues,
                queryValues: source.QueryValues,
                content: source.Content,
                contentType: source.ContentType,
                contentEncoding: source.ContentEncoding,
                timeout: source.Timeout);
        }
    }
}
