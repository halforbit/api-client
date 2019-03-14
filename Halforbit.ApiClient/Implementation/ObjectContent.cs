using System.IO;

namespace Halforbit.ApiClient
{
    public class ObjectContent<TObject> : IContent
    {
        readonly TObject _source;

        readonly ISerializer _serializer;

        public ObjectContent(
            TObject source,
            ISerializer serializer)
        {
            _source = source;

            _serializer = serializer;
        }

        public Stream GetStream()
        {
            var ms = new MemoryStream();

            _serializer.Serialize(_source, ms);

            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }
    }
}
