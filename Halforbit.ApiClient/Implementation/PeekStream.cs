using System;
using System.IO;

namespace Halforbit.ApiClient
{
    public class PeekStream : Stream, IDisposable
    {
        readonly Stream _sourceStream;

        readonly int _maxPeekLength;

        MemoryStream _peekStream;

        long _position;

        public PeekStream(Stream sourceStream, int maxPeekLength)
        {
            _sourceStream = sourceStream;

            _maxPeekLength = maxPeekLength;
        }

        private void PeekData(Stream source)
        {
            _peekStream = new MemoryStream();

            var bytesPeeked = ReadUpTo(
                source,
                _peekStream,
                _maxPeekLength);

            _peekStream.Seek(0, SeekOrigin.Begin);
        }

        public byte[] PeekedData => _peekStream.ToArray();

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => _position;

            set => throw new NotSupportedException();
        }

        public override void Close() => _sourceStream.Close();

        void IDisposable.Dispose() => _sourceStream.Dispose();

        public override void Flush() => throw new NotSupportedException();

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesRead = 0;

            if (_position < _peekStream.Length)
            {
                bytesRead = _peekStream.Read(buffer, offset, count);
            }
            else
            {
                bytesRead = _sourceStream.Read(buffer, offset, count);
            }

            _position += bytesRead;

            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        static int ReadUpTo(
            Stream source,
            Stream dest,
            int maxLength)
        {
            var buffer = new byte[4096];

            var offset = 0;

            var totalBytesRead = 0;

            while (offset < maxLength)
            {
                var bytesRead = source.Read(buffer, offset, maxLength - offset);

                totalBytesRead += bytesRead;

                if (bytesRead == 0) break;

                offset += bytesRead;
            }

            dest.Flush();

            return totalBytesRead;
        }
    }
}
