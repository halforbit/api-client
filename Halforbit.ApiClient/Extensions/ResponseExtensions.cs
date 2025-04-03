using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace Halforbit.ApiClient
{
    public static class ResponseExtensions
    {
        static readonly Encoding _utf8Encoding = new UTF8Encoding(false);

        static readonly Encoding _iso_8859_1_Encoding = Encoding.GetEncoding(28591);

        public static byte[] ByteContent(
            this Response response)
        {
            if (response.Request.DefaultContentStatusCodes.Contains(response.StatusCode))
            {
                return default;
            }

            if (response.Content is BufferedContent bufferedContent)
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
            if (response.Request.DefaultContentStatusCodes.Contains(response.StatusCode))
            {
                return default;
            }

            return response.Content.GetStream();
        }

        public static string TextContent(
            this Response response)
        {
            if (response.Request.DefaultContentStatusCodes.Contains(response.StatusCode))
            {
                return default;
            }

            var encoding = _utf8Encoding;

            if (response.ContentType?.Value != null)
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

        public static bool ContentIsText(
            this Response response)
        {
            var mediaType = response.ContentType?.MediaType ?? string.Empty;

            return mediaType.StartsWith("text/") || mediaType == "application/json";
        }

        public static TContent Content<TContent>(this Response response)
        {
            if (response.Request.DefaultContentStatusCodes.Contains(response.StatusCode))
            {
                return default;
            }

            return response.Request.Services.ResponseDeserializer
                .Deserialize<TContent>(response.Content.GetStream());
        }

        public static Response Content(
            this Response response, 
            IContent content)
        {
            return new Response(
                statusCode: response.StatusCode,
                headers: response.Headers,
                content: content,
                contentType: response.ContentType,
                contentEncoding: response.ContentEncoding,
                isSuccess: response.IsSuccess,
                errorMessage: response.ErrorMessage,
                exception: response.Exception,
                request: response.Request,
                requestedUrl: response.RequestedUrl);
        }

        public static (Response Source, Response Peeked) PeekContent(
            this Response response,
            int maxPeekLength = 1_024_000)
        {
            if(response.Content is StreamedContent streamedContent)
            {
                var peekStream = new PeekStream(
                    streamedContent.GetStream(), 
                    maxPeekLength);

                return (
                    response.Content(new StreamedContent(peekStream)),
                    response.Content(new BufferedContent(peekStream.PeekedData)));
            }
            else
            {
                return (response, response);
            }
        }

        public static TResult MapContent<TResult>(
            this Response response,
            Func<JsonNode, TResult> map)
        {
            var content = !response.Request.DefaultContentStatusCodes.Contains(response.StatusCode) ?
                response.Content<JsonNode>() :
                null;

            return map(content);
        }

        public static IReadOnlyList<TResult> MapContentArray<TResult>(
            this Response response,
            Func<JsonNode, TResult> map)
        {
            if (response.Request.DefaultContentStatusCodes.Contains(response.StatusCode))
            {
                return [];
            }

            return response.Content<JsonArray>().Select(t => map(t)).ToList();
        }
    }
}
