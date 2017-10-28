using System.Collections.Generic;
using Kernel.Data;
using ORMMetadataContextProvider.Models.GlobalConfiguration;

namespace ORMMetadataContextProvider.Models
{
    public class FederationPartySettings : BaseModel
    {
        public FederationPartySettings()
        {
            this.CertificatePins = new List<CertificatePin>();
        }

        public string FederationPartyId { get; set; }
        public string MetadataPath { get; set; }
        public string MetadataLocation { get; set; }
        public virtual DatepartValue AutoRefreshInterval { get; set; }
        public virtual DatepartValue RefreshInterval { get; set; }
        public virtual SecuritySettings SecuritySettings { get; set; }
        public virtual MetadataSettings MetadataSettings { get; set; }
        public virtual AutnRequestSettings AutnRequestSettings { get; set; }
        public virtual ICollection<CertificatePin> CertificatePins { get; }
    }
}