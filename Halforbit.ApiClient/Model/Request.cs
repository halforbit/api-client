using System;
using System.Collections.Generic;

namespace Halforbit.ApiClient
{
    public partial class Request
    {
        static readonly IReadOnlyDictionary<string, string> _emptyDictionary = new Dictionary<string, string>(0);

        public Request(
            IRequestClient requestClient,
            IAuthenticationStrategy authenticationStrategy,
            IRetryStrategy retryStrategy,
            string baseUrl,
            string method,
            string resource,
            IReadOnlyDictionary<string, string> headers,
            IReadOnlyDictionary<string, string> routeValues,
            IReadOnlyDictionary<string, string> queryValues,
            IReadOnlyList<byte> content,
            string contentType,
            string contentEncoding,
            TimeSpan timeout)
        {
            RequestClient = requestClient;

            AuthenticationStrategy = authenticationStrategy;

            RetryStrategy = retryStrategy;

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

        public IRequestClient RequestClient { get; }

        public IAuthenticationStrategy AuthenticationStrategy { get; }

        public IRetryStrategy RetryStrategy { get; }

        public string BaseUrl { get; }

        public string Method { get; }

        public string Resource { get; }

        public IReadOnlyDictionary<string, string> Headers { get; }

        public IReadOnlyDictionary<string, string> RouteValues { get; }

        public IReadOnlyDictionary<string, string> QueryValues { get; }

        public IReadOnlyList<byte> Content { get; }

        public string ContentType { get; }

        public string ContentEncoding { get; }

        public TimeSpan Timeout { get; }

        public static Request Default => new Request(
            requestClient: Halforbit.ApiClient.RequestClient.Instance,
            authenticationStrategy: default,
            retryStrategy: default,
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
                requestClient: requestClient ?? source.RequestClient,
                authenticationStrategy: source.AuthenticationStrategy,
                retryStrategy: source.RetryStrategy,
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
