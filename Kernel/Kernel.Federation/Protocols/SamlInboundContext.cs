using System.Collections.Generic;
using System.IdentityModel.Metadata;
using Kernel.Federation.Protocols.Bindings;

namespace Kernel.Federation.Protocols
{
    public class SamlInboundContext
    {
        public SamlInboundContext()
        {
            this.Keys = new List<KeyDescriptor>();
        }
        public ICollection<KeyDescriptor> Keys { get; }
        public object RelayState { get; set; }
        public string Request { get; set; }
        public SamlInboundMessage Message { get; set; }
    }
}