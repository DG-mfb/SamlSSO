using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;

namespace Kernel.Federation.Protocols.Bindings.HttpRedirectBinding
{
    public class HttpRedirectInboundContext : SamlInboundContext
    {
        public HttpRedirectInboundContext()
        {
            this.Keys = new List<KeyDescriptor>();
        }
        public string Request { get; set; }
        public IDictionary<string, string> Form { get; set; }
        public ICollection<KeyDescriptor> Keys { get; }
        public Action HanlerAction { get; set; }
    }
}