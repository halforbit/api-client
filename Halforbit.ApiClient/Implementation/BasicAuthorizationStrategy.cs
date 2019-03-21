using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public class BasicAuthorizationStrategy : IAuthorizationStrategy
    {
        readonly string _username;

        readonly string _password;

        public BasicAuthorizationStrategy(
            string username,
            string password)
        {
            _username = username;

            _password = password;
        }

        public bool ShouldReauthorize(HttpStatusCode httpStatusCode) => false;

        public Task<Request> Apply(Request request)
        {
            var encoded = Convert.ToBase64String(Encoding
                .GetEncoding("ISO-8859-1")
                .GetBytes($"{_username}:{_password}"));

            return Task.FromResult(request.Header("Authorization", $"Basic {encoded}"));
        }

        public Task Reauthorize()
        {
            throw new NotSupportedException();
        }
    }
}
