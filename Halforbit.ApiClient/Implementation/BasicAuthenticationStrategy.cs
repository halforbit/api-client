using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public class BasicAuthenticationStrategy : IAuthenticationStrategy
    {
        readonly string _username;

        readonly string _password;

        public BasicAuthenticationStrategy(
            string username,
            string password)
        {
            _username = username;

            _password = password;
        }

        public bool ShouldReauthenticate(HttpStatusCode httpStatusCode) => false;

        public Task<Request> Apply(Request request)
        {
            var encoded = Convert.ToBase64String(Encoding
                .GetEncoding("ISO-8859-1")
                .GetBytes($"{_username}:{_password}"));

            return Task.FromResult(request.Header("Authorization", $"Basic {encoded}"));
        }

        public Task Reauthenticate()
        {
            throw new NotSupportedException();
        }
    }
}
