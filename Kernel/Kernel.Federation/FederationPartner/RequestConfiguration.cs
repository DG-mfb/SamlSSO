using System.Collections.Generic;
using Kernel.Federation.MetaData.Configuration.EntityDescriptors;
using Kernel.Federation.MetaData.Configuration.RoleDescriptors;

namespace Kernel.Federation.FederationPartner
{
    public class RequestConfiguration
    {
        private readonly EntityDesriptorConfiguration _entityDesriptorConfiguration;
        public RequestConfiguration(string requestId, string version, EntityDesriptorConfiguration entityDesriptorConfiguration)
        {
            this.RequestId = requestId;
            this.Version = version;
            this._entityDesriptorConfiguration = entityDesriptorConfiguration;
        }
        
        public string RequestId { get; }
       
        public string Version { get; }
        public string EntityId { get { return this._entityDesriptorConfiguration.EntityId; } }

        public ICollection<SPSSODescriptorConfiguration> SPSSODescriptors
        {
            get
            {
                return this._entityDesriptorConfiguration.SPSSODescriptors;
            }
        }
    }
}