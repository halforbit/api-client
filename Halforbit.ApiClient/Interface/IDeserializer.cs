using System.IO;

namespace Halforbit.ApiClient
{
    public interface IDeserializer
    {
        TValue Deserialize<TValue>(Stream stream);
    }
}
