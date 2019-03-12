using System;
using System.Net;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public class CookieAuthenticationStrategy : IAuthenticationStrategy
    {
        readonly Func<Task<IAuthenticationToken>> _getCookie;

        IAuthenticationToken _authenticationToken;

        public CookieAuthenticationStrategy(
            Func<Task<IAuthenticationToken>> getCookie)
        {
            _getCookie = getCookie;
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
                _authenticationToken = await _getCookie();
            }

            return request.Header("Cookie", _authenticationToken.Content);
        }

        public async Task Reauthenticate()
        {
            _authenticationToken = await _getCookie();
        }
    }
}
