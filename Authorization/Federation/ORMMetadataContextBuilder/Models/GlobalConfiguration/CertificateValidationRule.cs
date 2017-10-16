using Kernel.Data;
using Kernel.Security.Validation;

namespace ORMMetadataContextProvider.Models.GlobalConfiguration
{
    public class CertificateValidationRule : BaseModel
    {
        public string Type { get; set; }
        public virtual ValidationScope Scope { get; set; }
    }
}