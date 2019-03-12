using System;
using System.Net;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public class BearerTokenAuthenticationStrategy : IAuthenticationStrategy
    {
        readonly Func<Task<IAuthenticationToken>> _getAuthenticationToken;

        IAuthenticationToken _authenticationToken;

        public BearerTokenAuthenticationStrategy(
            Func<Task<IAuthenticationToken>> getAuthenticationToken)
        {
            _getAuthenticationToken = getAuthenticationToken;
        }

        public bool ShouldReauthenticate(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.Unauthorized;
        }

        public async Task<Request> Apply(Request request)
        {
            var expireTime = _authenticationToken?.ExpireTime;

            if (_authenticationToken == null ||
                (expireTime.HasValue && expireTime.Value < DateTime.UtcNow))
            { 
                _authenticationToken = await _getAuthenticationToken();
            }

            return request.Header("Authentication", $"Bearer {_authenticationToken.Content}");
        }

        public async Task Reauthenticate()
        {
            _authenticationToken = await _getAuthenticationToken();
        }
    }
}
