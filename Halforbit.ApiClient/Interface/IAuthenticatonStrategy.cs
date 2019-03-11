using System.Net;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public interface IAuthenticationStrategy
    {
        bool ShouldReauthenticate(HttpStatusCode statusCode);

        Task<Request> Apply(Request request);

        Task Reauthenticate();
    }
}
