using Kernel.Federation.MetaData.Configuration.EntityDescriptors;

namespace Kernel.Federation.FederationPartner
{
    public class RequestConfiguration
    {
        private readonly EntityDesriptorConfiguration _entityDesriptorConfiguration;
        public RequestConfiguration(string requestId, string version)
        {
            this.RequestId = requestId;
            this.Version = version;
        }
        
        public string RequestId { get; }
       
        public string Version { get; }
       
    }
}