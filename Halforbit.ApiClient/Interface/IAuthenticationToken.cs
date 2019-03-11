using System;

namespace Halforbit.ApiClient
{
    public interface IAuthenticationToken
    {
        string Content { get; }

        DateTime? ExpireTime { get; }
    }
}
