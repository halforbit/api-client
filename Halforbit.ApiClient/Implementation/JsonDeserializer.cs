using System.IO;

namespace Halforbit.ApiClient
{
    public class JsonDeserializer : IDeserializer
    {
        public static IDeserializer Instance => new JsonDeserializer();

        public TValue Deserialize<TValue>(Stream stream)
        {
            return System.Text.Json.JsonSerializer.Deserialize<TValue>(stream);
        }
    }
}
