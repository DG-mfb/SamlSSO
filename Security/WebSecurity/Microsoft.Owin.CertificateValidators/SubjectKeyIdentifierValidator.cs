using System.Collections.Generic;
using Kernel.Cryptography.Validation;
using Microsoft.Owin.Security;

namespace Microsoft.Owin.CertificateValidators
{
    internal class SubjectKeyIdentifierValidator : CertificateSubjectKeyIdentifierValidator, IBackchannelCertificateValidator
    {
        public SubjectKeyIdentifierValidator(IEnumerable<string> validSubjectKeyIdentifiers) : base(validSubjectKeyIdentifiers)
        {
        }
    }
}