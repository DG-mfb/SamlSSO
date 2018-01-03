using System.Collections.Generic;
using System.IdentityModel.Metadata;

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
        public IDictionary<string, string> Form { get; set; }
    }
}