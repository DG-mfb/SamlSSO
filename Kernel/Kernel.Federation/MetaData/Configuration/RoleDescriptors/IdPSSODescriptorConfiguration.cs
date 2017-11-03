using System.Collections.Generic;
using Kernel.Federation.MetaData.Configuration.EndPoint;

namespace Kernel.Federation.MetaData.Configuration.RoleDescriptors
{
    public class IdPSSODescriptorConfiguration : SSODescriptorConfiguration
    {
        public bool WantAuthenticationRequestsSigned { get; set; }

        public ICollection<EndPointConfiguration> SignOnServices { get; }
        public IdPSSODescriptorConfiguration()
        {
            this.WantAuthenticationRequestsSigned = true;
            this.SignOnServices = new List<EndPointConfiguration>();
        }
    }
}