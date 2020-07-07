using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public class BearerTokenAuthorizationWithBaseUrlStrategy : IAuthorizationStrategy
    {
        readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        readonly Func<Task<(IAuthorizationToken BearerToken, string BaseUrl)>> _getAuthorizationTokenWithBaseUrl;

        IAuthorizationToken _authorizationToken;

        string _baseUrl;

        public BearerTokenAuthorizationWithBaseUrlStrategy(
            Func<Task<(IAuthorizationToken BearerToken, string BaseUrl)>> getAuthorizationTokenWithBaseUrl)
        {
            _getAuthorizationTokenWithBaseUrl = getAuthorizationTokenWithBaseUrl;
        }

        public bool ShouldReauthorize(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.Unauthorized;
        }

        public async Task<Request> Apply(Request request)
        {
            if (!AuthorizationTokenValid)
            {
                try
                {
                    await _semaphore.WaitAsync();

                    if (!AuthorizationTokenValid)
                    {
                        (_authorizationToken, _baseUrl) = await _getAuthorizationTokenWithBaseUrl();
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return request
                .BaseUrl(_baseUrl)
                .Header("Authorization", $"Bearer {_authorizationToken.Content}");
        }

        bool AuthorizationTokenValid => _authorizationToken != null &&
            (!_authorizationToken.ExpireTime.HasValue || _authorizationToken.ExpireTime.Value > DateTime.UtcNow);

        public async Task Reauthorize()
        {
            try
            {
                await _semaphore.WaitAsync();

                (_authorizationToken, _baseUrl) = await _getAuthorizationTokenWithBaseUrl();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
