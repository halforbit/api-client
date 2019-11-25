using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Halforbit.ApiClient
{
    public class ExponentialBackoffRetryStrategy : IRetryStrategy
    {
        static readonly IReadOnlyCollection<HttpStatusCode> _retryableStatusCodes =
            new HashSet<HttpStatusCode>
            {
                0,

                HttpStatusCode.InternalServerError,

                HttpStatusCode.BadGateway,

                HttpStatusCode.GatewayTimeout,

                HttpStatusCode.RequestTimeout,

                HttpStatusCode.ServiceUnavailable
            };

        public ExponentialBackoffRetryStrategy(
            int retryCount,
            bool retryOnTimeout)
        {
            RetryCount = retryCount;

            ShouldRetryOnTimeout = retryOnTimeout;
        }

        public int RetryCount { get; }

        public bool ShouldRetryOnTimeout { get; }

        public bool ShouldRetry(HttpStatusCode statusCode) => _retryableStatusCodes.Contains(statusCode);

        public TimeSpan GetRetryTimeout(int attemptCount)
        {
            if (attemptCount == 1) return TimeSpan.Zero;

            return TimeSpan.FromSeconds(Math.Pow(2, attemptCount - 1));
        }
    }
}
