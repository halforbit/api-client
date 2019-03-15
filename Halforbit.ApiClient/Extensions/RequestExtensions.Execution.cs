using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Halforbit.ApiClient
{
    public static partial class RequestExtensions
    {
        public static async Task<Response> ExecuteAsync(this Request request)
        {
            return await request.Services.RequestClient.ExecuteAsync(request);
        }

        public static async Task<Response> GetAsync(
            this Request request,
            string resource = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("GET")
                .ExecuteAsync();
        }

        public static async Task<Response> PostAsync(
            this Request request,
            string resource = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("POST")
                .ExecuteAsync();
        }

        public static async Task<Response> PutAsync(
            this Request request,
            string resource = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("PUT")
                .ExecuteAsync();
        }

        public static async Task<Response> PatchAsync(
            this Request request,
            string resource = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("PATCH")
                .ExecuteAsync();
        }

        public static async Task<Response> DeleteAsync(
            this Request request,
            string resource = default)
        {
            return await (resource == null ? request : request.Resource(resource))
                .Method("DELETE")
                .ExecuteAsync();
        }
    }
}
