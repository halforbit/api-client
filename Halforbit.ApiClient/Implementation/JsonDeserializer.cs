using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Halforbit.ApiClient
{
    public class JsonDeserializer : IDeserializer
    {
        static readonly Encoding _utf8Encoding = new UTF8Encoding(false);

        public static IDeserializer Instance => new JsonDeserializer();

        public TValue Deserialize<TValue>(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                return JsonConvert.DeserializeObject<TValue>(sr.ReadToEnd());
            }
        }
    }
}
