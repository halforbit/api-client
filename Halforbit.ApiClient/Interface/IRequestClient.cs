using System.Threading;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public interface IRequestClient
    {
        Task<Response> ExecuteAsync(Request request, CancellationToken cancellationToken = default);
    }
}
