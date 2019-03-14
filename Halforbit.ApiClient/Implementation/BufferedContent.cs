using System.IO;

namespace Halforbit.ApiClient
{
    public class BufferedContent : IContent
    {
        public BufferedContent(byte[] bytes)
        {
            Bytes = bytes;
        }

        public byte[] Bytes { get; }

        public Stream GetStream()
        {
            return new MemoryStream(Bytes);
        }
    }
}
