using System;

namespace Halforbit.ApiClient
{
    public class AuthenticationToken : IAuthenticationToken
    {
        public AuthenticationToken(
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
