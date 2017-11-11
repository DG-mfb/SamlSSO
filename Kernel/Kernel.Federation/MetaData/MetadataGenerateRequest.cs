using System.IO;

namespace Kernel.Federation.MetaData
{
    public class MetadataGenerateRequest
    {
        public MetadataGenerateRequest(MetadataType type, string federationPartyId)
            :this(type, federationPartyId, new MetadataPublicationContext(new MemoryStream(), MetadataPublicationProtocol.Memory))
        {
            
        }
        public MetadataGenerateRequest(MetadataType type, string federationPartyId, MetadataPublicationContext target)
        {
            this.MetadataType = type;
            this.FederationPartyId = federationPartyId;
            this.Target = target;
        }

        public MetadataType MetadataType { get; }
        public string FederationPartyId { get; set; }
        public MetadataPublicationContext Target { get; }
    }
}