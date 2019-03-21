using System;

namespace Halforbit.ApiClient
{
    public class AuthorizationToken : IAuthorizationToken
    {
        public AuthorizationToken(
            string content,
            DateTime? expireTime)
        {
            Content = content;

            ExpireTime = expireTime;
        }

        public string Content { get; }

        public DateTime? ExpireTime { get; }
    }
}
