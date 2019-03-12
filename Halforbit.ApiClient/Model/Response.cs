using System;
using System.Collections.Generic;
using System.Net;

namespace Halforbit.ApiClient
{
    public class Response
    {
        public Response(
            HttpStatusCode statusCode,
            IReadOnlyDictionary<string, string> headers,
            IReadOnlyList<byte> content,
            ContentType contentType,
            string contentEncoding,
            bool isSuccess,
            string errorMessage,
            Exception exception,
            Request request,
            string requestedUrl)
        {
            StatusCode = statusCode;

            Headers = headers;

            Content = content;

            ContentType = contentType;

            ContentEncoding = contentEncoding;

            IsSuccess = isSuccess;

            ErrorMessage = errorMessage;

            Exception = exception;

            Request = request;

            RequestedUrl = requestedUrl;
        }

        public HttpStatusCode StatusCode { get; }

        public IReadOnlyDictionary<string, string> Headers { get; }

        public IReadOnlyList<byte> Content { get; }

        public ContentType ContentType { get; }

        public string ContentEncoding { get; }

        public bool IsSuccess { get; }
        
        public string ErrorMessage { get; }

        public Exception Exception { get; }

        public Request Request { get; }

        public string RequestedUrl { get; }
    }
}
