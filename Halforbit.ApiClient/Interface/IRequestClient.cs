using System;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public interface IRequestClient : IDisposable
    {
        Task<Response> ExecuteAsync(Request request);
    }
}
