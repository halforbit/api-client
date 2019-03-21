using System;
using System.Net;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public class BearerTokenAuthorizationStrategy : IAuthorizationStrategy
    {
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
            var expireTime = _authorizationToken?.ExpireTime;

            if (_authorizationToken == null ||
                (expireTime.HasValue && expireTime.Value < DateTime.UtcNow))
            { 
                _authorizationToken = await _getAuthorizationToken();
            }

            return request.Header("Authorization", $"Bearer {_authorizationToken.Content}");
        }

        public async Task Reauthorize()
        {
            _authorizationToken = await _getAuthorizationToken();
        }
    }
}
