using System;

namespace Halforbit.ApiClient
{
    public interface IAuthorizationToken
    {
        string Content { get; }

        DateTime? ExpireTime { get; }
    }
}
