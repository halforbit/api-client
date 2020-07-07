using System.IO;
using System.Text;

namespace Halforbit.ApiClient.Implementation
{
    public class XmlSerializer : ISerializer
    {
        static readonly Encoding _utf8Encoding = new UTF8Encoding(false);

        public static ISerializer Instance => new XmlSerializer();

        public string ContentType => "application/xml; charset=utf-8";

        public void Serialize<TValue>(TValue value, Stream stream)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(TValue));

            using (var streamWriter = new StreamWriter(stream, _utf8Encoding, 1024, true))
            {
                xmlSerializer.Serialize(
                    streamWriter,
                    value);
            }
        }
    }
}
