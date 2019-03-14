using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Halforbit.ApiClient
{
    public static class ResponseExtensions
    {
        static readonly Encoding _utf8Encoding = new UTF8Encoding(false);

        static readonly Encoding _iso_8859_1_Encoding = Encoding.GetEncoding(28591);

        public static byte[] ByteContent(
            this Response response)
        {
            if(response.Content is BufferedContent bufferedContent)
            {
                return bufferedContent.Bytes;
            }

            using (var ms = new MemoryStream())
            {
                response.Content.GetStream().CopyTo(ms);

                return ms.ToArray();
            }
        }

        public static Stream StreamContent(
            this Response response)
        {
            return response.Content.GetStream();
        }

        public static string TextContent(
            this Response response)
        {
            var encoding = _utf8Encoding;

            if (response.ContentType != null)
            {
                var charSet = (response.ContentType.CharSet ?? string.Empty).ToLower();

                switch (charSet)
                {
                    case "ascii":
                    case "us-ascii":
                        encoding = Encoding.ASCII;
                        break;

                    case "":
                    case "utf-8":
                        // Already set.
                        break;

                    case "iso-8859-1":
                        encoding = _iso_8859_1_Encoding;
                        break;

                    default: throw new Exception($"Unhandled charset '{response.ContentType.CharSet}'");
                }
            }

            using (var sr = new StreamReader(response.Content.GetStream(), encoding))
            {
                return sr.ReadToEnd();
            }
        }

        public static TContent Content<TContent>(this Response response)
        {
            return response.Request.ResponseDeserializer
                .Deserialize<TContent>(response.Content.GetStream());
        }

        public static TResult MapContent<TResult>(
            this Response response,
            Func<JToken, TResult> map)
        {
            return map(response.Content<JToken>());
        }

        public static IReadOnlyList<TResult> MapContentArray<TResult>(
            this Response response,
            Func<JToken, TResult> map)
        {
            return response.Content<JArray>().Select(t => map(t)).ToList();
        }
    }
}
