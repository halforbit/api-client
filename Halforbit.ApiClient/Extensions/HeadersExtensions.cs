using System;
using System.Collections.Generic;

namespace Halforbit.ApiClient
{
    public static class HeadersExtensions
    {
        public static string ETag(this IReadOnlyDictionary<string, string> headers)
        {
            return headers.TryGetValue("ETag", out var value) ? value : null;
        }

        public static DateTime? Date(this IReadOnlyDictionary<string, string> headers)
        {
            return headers.TryGetValue("Date", out var value) ? 
                DateTime.TryParse(value, out var date) ? date : default(DateTime?) : 
                default;
        }
    }
}
