using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Halforbit.ApiClient
{
    public class JsonSerializer : ISerializer
    {
        static readonly Encoding _utf8Encoding = new UTF8Encoding(false);

        public static ISerializer Instance => new JsonSerializer();

        public string ContentType => "application/json; charset=utf-8";

        public void Serialize<TValue>(TValue value, Stream stream)
        {
            var json = JsonConvert.SerializeObject(value);

            using (var sw = new StreamWriter(stream, _utf8Encoding, 1024, true))
            {
                sw.Write(json);
            }
        }
    }
}
