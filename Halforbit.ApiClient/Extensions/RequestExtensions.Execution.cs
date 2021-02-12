using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public static partial class RequestExtensions
    {
        public static async Task<Response> ExecuteAsync(
            this Request request,
            CancellationToken cancellationToken = default)
        {
            return await request.Services.RequestClient.ExecuteAsync(request, cancellationToken);
        }

        public static async Task<Response> GetAsync(
            this Request request,
            string resource = default,
            CancellationToken cancellationToken = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("GET")
                .ExecuteAsync(cancellationToken);
        }

        public static async Task<Response> PostAsync(
            this Request request,
            string resource = default,
            CancellationToken cancellationToken = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("POST")
                .ExecuteAsync(cancellationToken);
        }

        public static async Task<Response> PutAsync(
            this Request request,
            string resource = default,
            CancellationToken cancellationToken = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("PUT")
                .ExecuteAsync(cancellationToken);
        }

        public static async Task<Response> PatchAsync(
            this Request request,
            string resource = default,
            CancellationToken cancellationToken = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("PATCH")
                .ExecuteAsync(cancellationToken);
        }

        public static async Task<Response> DeleteAsync(
            this Request request,
            string resource = default,
            CancellationToken cancellationToken = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("DELETE")
                .ExecuteAsync(cancellationToken);
        }

        public static async Task<Response> HeadAsync(
            this Request request,
            string resource = default,
            CancellationToken cancellationToken = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("HEAD")
                .ExecuteAsync(cancellationToken);
        }
    }
}
