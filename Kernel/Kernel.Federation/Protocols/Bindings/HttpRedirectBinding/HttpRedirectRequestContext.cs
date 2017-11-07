using System;
using System.Threading.Tasks;

namespace Kernel.Federation.Protocols.Bindings.HttpRedirectBinding
{
    public class HttpRedirectRequestContext : SamlOutboundContext
    {
        public Func<Uri, Task> RequestHanlerAction { get; set; }
    }
}