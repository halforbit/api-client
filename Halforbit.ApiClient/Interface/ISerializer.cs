using System.IO;

namespace Halforbit.ApiClient
{
    public interface ISerializer
    {
        void Serialize<TValue>(TValue value, Stream stream);

        string ContentType { get; }
    }
}
