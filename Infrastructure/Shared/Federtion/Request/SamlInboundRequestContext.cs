using System.Collections.Generic;
using System.IdentityModel.Metadata;
using Kernel.Federation.Protocols;
using Shared.Federtion.Models;

namespace Shared.Federtion.Request
{
    public class SamlInboundRequestContext
    {
        public SamlInboundRequestContext()
        {
            this.Keys = new List<KeyDescriptor>();
        }
        public RequestAbstract SamlRequest { get; set; }   
        public string Request { get; set; }
        public SamlInboundMessage SamlInboundMessage { get; set; }
        public ICollection<KeyDescriptor> Keys { get; }
    }
}