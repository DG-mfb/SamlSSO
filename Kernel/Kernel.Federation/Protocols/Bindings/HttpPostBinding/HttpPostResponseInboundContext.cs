using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Kernel.Federation.Protocols.Bindings.HttpPostBinding
{
    public class HttpPostResponseInboundContext : SamlInboundContext
    {
        public HttpPostResponseInboundContext()
        {
            this.Properties = new Dictionary<string, object>();
        }
        public string AuthenticationMethod { get; set; }
        public Uri RequestUri { get; set; }
        public ClaimsIdentity Identity { get; set; }
        public bool HasSecurityToken { get; set; }
        public IDictionary<string, object> Properties { get; }
    }
}