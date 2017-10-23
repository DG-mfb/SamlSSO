using Kernel.Data;
using Kernel.Security.Configuration;

namespace ORMMetadataContextProvider.Models.GlobalConfiguration
{
    public class CertificatePin : BaseModel
    {
        public PinType PinType { get; set; }
        public string Value { get; set; }
        public string Algorithm { get; set; }
    }
}