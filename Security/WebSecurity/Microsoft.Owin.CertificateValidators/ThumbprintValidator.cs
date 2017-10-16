using System.Collections.Generic;
using Kernel.Cryptography.Validation;
using Microsoft.Owin.Security;

namespace Microsoft.Owin.CertificateValidators
{
    internal class ThumbprintValidator : CertificateThumbprintValidator, IBackchannelCertificateValidator
    {
        public ThumbprintValidator(IEnumerable<string> validThumbprints) : base(validThumbprints)
        {
        }
    }
}