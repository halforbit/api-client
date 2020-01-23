using System.IO;

namespace Halforbit.ApiClient.Implementation
{
    public class XmlDeserializer : IDeserializer
    {
        public static IDeserializer Instance => new XmlDeserializer();

        public TValue Deserialize<TValue>(Stream stream)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(TValue));

            return (TValue)xmlSerializer.Deserialize(stream);
        }
    }
}
