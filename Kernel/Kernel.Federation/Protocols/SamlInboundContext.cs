using Kernel.Federation.Constants;
using System;
using System.IdentityModel.Metadata;

namespace Kernel.Federation.Protocols
{
    public class SamlInboundContext
    {
        public object RelayState
        {
            get
            {
                if (this.Message != null && this.Message.Elements.ContainsKey(HttpRedirectBindingConstants.RelayState))
                    return this.Message.Elements[HttpRedirectBindingConstants.RelayState];
                return null;
            }
        }
        public SamlInboundMessage Message { get; set; }
        public Func<MetadataBase, RoleDescriptor> DescriptorResolver { get; set; }
    }
}