using System;
using System.Net;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public class CookieAuthorizationStrategy : IAuthorizationStrategy
    {
        readonly Func<Task<IAuthorizationToken>> _getCookie;

        IAuthorizationToken _authorizationToken;

        public CookieAuthorizationStrategy(
            Func<Task<IAuthorizationToken>> getCookie)
        {
            _getCookie = getCookie;
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
                _authorizationToken = await _getCookie();
            }

            return request.Header("Cookie", _authorizationToken.Content);
        }

        public async Task Reauthorize()
        {
            _authorizationToken = await _getCookie();
        }
    }
}
