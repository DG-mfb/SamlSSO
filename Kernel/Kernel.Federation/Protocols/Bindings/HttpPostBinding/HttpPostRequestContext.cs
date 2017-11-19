using System;
using System.Threading.Tasks;

namespace Kernel.Federation.Protocols.Bindings.HttpPostBinding
{
    public class HttpPostRequestContext : SamlOutboundContext<string>
    {
        public override Func<string, Task> DespatchDelegate { get; set; }
    }
}