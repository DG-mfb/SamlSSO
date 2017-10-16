using System.Collections.Generic;
using Kernel.Security.Validation;
using Microsoft.Owin.Security;

namespace Microsoft.Owin.CertificateValidators
{
    internal class SubjectPublicKeyInfoValidator : CertificateSubjectPublicKeyInfoValidator, IBackchannelCertificateValidator
    {
        public SubjectPublicKeyInfoValidator(IEnumerable<string> validBase64EncodedSubjectPublicKeyInfoHashes, SubjectPublicKeyInfoAlgorithm algorithm)
            : base(validBase64EncodedSubjectPublicKeyInfoHashes, algorithm)
        {
        }
    }
}