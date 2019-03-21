using System.Net;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public interface IAuthorizationStrategy
    {
        bool ShouldReauthorize(HttpStatusCode statusCode);

        Task<Request> Apply(Request request);

        Task Reauthorize();
    }
}
