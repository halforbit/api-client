using System;
using System.Net;

namespace Halforbit.ApiClient
{
    public interface IRetryStrategy
    {
        int RetryCount { get; }

        bool ShouldRetryOnTimeout { get; }

        bool ShouldRetry(HttpStatusCode statusCode);

        TimeSpan GetRetryTimeout(int attemptCount);
    }
}
