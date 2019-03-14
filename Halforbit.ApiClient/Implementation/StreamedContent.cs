using System;
using System.IO;

namespace Halforbit.ApiClient
{
    public class StreamedContent : IContent, IDisposable
    {
        readonly Stream _stream;

        public StreamedContent(Stream stream)
        {
            _stream = stream;
        }

        public void Dispose() => _stream.Dispose();

        public Stream GetStream()
        {
            return _stream;
        }
    }
}
