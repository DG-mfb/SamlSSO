using System;
using System.Threading.Tasks;

namespace Kernel.Federation.Protocols.Bindings.HttpPostBinding
{
    public class HttpPosttRequestContext : SamlOutboundContext
    {
        public Func<string, Task> HanlerAction { get; set; }
    }
}