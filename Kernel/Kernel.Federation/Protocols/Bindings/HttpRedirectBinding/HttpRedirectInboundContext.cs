using System;

namespace Kernel.Federation.Protocols.Bindings.HttpRedirectBinding
{
    public class HttpRedirectInboundContext : SamlInboundContext
    {
        public Action HanlerAction { get; set; }
    }
}