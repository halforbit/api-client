using System;

namespace Halforbit.ApiClient
{
    public class ApiRequestException : Exception
    {
        public ApiRequestException(
            string message,
            Request request,
            Response response) : base(message)
        {
            Request = request;

            Response = response;
        }

        public Request Request { get; }

        public Response Response { get; }
    }
}
