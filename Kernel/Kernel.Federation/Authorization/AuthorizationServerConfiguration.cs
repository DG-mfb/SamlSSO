using System;

namespace Kernel.Federation.Authorization
{
    public class AuthorizationServerConfiguration
    {
        public Uri TokenResponseUrl { get; set; }
        public bool CreateToken { get; set; }
    }
}