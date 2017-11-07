using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kernel.Federation.Protocols.Bindings.HttpRedirectBinding
{
    public class HttpRedirectInboundContext : SamlInboundContext
    {
        public string Request { get; set; }
        public IDictionary<string, string> Form { get; set; }
        public Action HanlerAction { get; set; }
    }
}