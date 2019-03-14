using System.IO;

namespace Halforbit.ApiClient
{
    public interface IContent
    {
        Stream GetStream();
    }
}
