using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public class BearerTokenAuthorizationStrategy : IAuthorizationStrategy
    {
        readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        readonly Func<Task<IAuthorizationToken>> _getAuthorizationToken;

        IAuthorizationToken _authorizationToken;

        public BearerTokenAuthorizationStrategy(
            Func<Task<IAuthorizationToken>> getAuthorizationToken)
        {
            _getAuthorizationToken = getAuthorizationToken;
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
                        _authorizationToken = await _getAuthorizationToken();
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return request.Header("Authorization", $"Bearer {_authorizationToken.Content}");
        }

        bool AuthorizationTokenValid => _authorizationToken != null &&
            (!_authorizationToken.ExpireTime.HasValue || _authorizationToken.ExpireTime.Value > DateTime.UtcNow);

        public async Task Reauthorize()
        {
            try
            {
                await _semaphore.WaitAsync();

                _authorizationToken = await _getAuthorizationToken();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
