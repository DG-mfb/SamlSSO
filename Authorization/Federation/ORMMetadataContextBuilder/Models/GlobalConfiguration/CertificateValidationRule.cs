using Kernel.Data;
using Kernel.Security.Configuration;

namespace ORMMetadataContextProvider.Models.GlobalConfiguration
{
    public class CertificateValidationRule : BaseModel
    {
        public string Type { get; set; }
        public virtual ValidationScope Scope { get; set; }
    }
}