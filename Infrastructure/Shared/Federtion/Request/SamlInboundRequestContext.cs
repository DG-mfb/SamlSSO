using Kernel.Federation.Protocols;
using Shared.Federtion.Models;

namespace Shared.Federtion.Request
{
    public class SamlInboundRequestContext
    {
        public RequestAbstract SamlRequest { get; set; }   
        public string Request { get; set; }
        public SamlInboundMessage SamlInboundMessage { get; set; }
    }
}