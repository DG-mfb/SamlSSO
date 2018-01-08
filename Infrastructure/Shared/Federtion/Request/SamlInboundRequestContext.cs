using Shared.Federtion.Models;

namespace Shared.Federtion.Request
{
    public class SamlInboundRequestContext : SamlInboundMessageContext
    {
        public RequestAbstract SamlRequest { get; set; }   
    }
}