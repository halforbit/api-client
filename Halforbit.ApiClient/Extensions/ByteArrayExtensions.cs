using System.Collections.Generic;
using System.Linq;

namespace Halforbit.ApiClient
{
    public static class ByteArrayExtensions
    {
        public static byte[] AsByteArray(this IReadOnlyList<byte> bytes)
        {
            if (bytes == null) return null;

            if (bytes is byte[]) return bytes as byte[];

            return bytes.ToArray();
        }
    }
}
