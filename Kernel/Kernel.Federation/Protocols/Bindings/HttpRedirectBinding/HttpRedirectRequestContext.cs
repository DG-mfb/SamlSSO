using System;
using System.Threading.Tasks;

namespace Kernel.Federation.Protocols.Bindings.HttpRedirectBinding
{
    public class HttpRedirectRequestContext : SamlOutboundContext<Uri>
    {
        public override Func<Uri, Task> DespatchDelegate { get; set; }
    }
}