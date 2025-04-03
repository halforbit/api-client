using System.IO;

namespace Halforbit.ApiClient
{
    public class JsonSerializer : ISerializer
    {
        public static ISerializer Instance => new JsonSerializer();

        public string ContentType => "application/json; charset=utf-8";

        public void Serialize<TValue>(TValue value, Stream stream)
        {
            System.Text.Json.JsonSerializer.Serialize(stream, value);
        }
    }
}
