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
            return response.Content.AsByteArray();
        }

        public static Stream StreamContent(
            this Response response)
        {
            return new MemoryStream(response.Content.AsByteArray());
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

            return encoding.GetString(response.Content.AsByteArray());
        }

        public static JToken JsonContent(this Response response)
        {
            return response.JsonContent<JToken>();
        }

        public static TValue JsonContent<TValue>(this Response response)
        {
            return JsonConvert.DeserializeObject<TValue>(response.TextContent());
        }

        public static TResult MapJsonContent<TResult>(
            this Response response,
            Func<JToken, TResult> map)
        {
            return map(response.JsonContent<JToken>());
        }

        public static IReadOnlyList<TResult> MapJsonArrayContent<TResult>(
            this Response response,
            Func<JToken, TResult> map)
        {
            return response.JsonContent<JArray>().Select(t => map(t)).ToList();
        }
    }
}
