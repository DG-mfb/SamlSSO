using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kernel.Security.Validation;
using Microsoft.Owin.Security;

namespace Microsoft.Owin.CertificateValidators
{
    internal class SubjectPublicKeyInfoValidator : CertificateSubjectPublicKeyInfoValidator, IPinningSertificateValidator
    {
        public SubjectPublicKeyInfoValidator(IEnumerable<string> validBase64EncodedSubjectPublicKeyInfoHashes, SubjectPublicKeyInfoAlgorithm algorithm)
            : base(validBase64EncodedSubjectPublicKeyInfoHashes, algorithm)
        {
        }

        public Task Validate(object sender, BackchannelCertificateValidationContext context, Func<object, BackchannelCertificateValidationContext, Task> next)
        {
            throw new NotImplementedException();
        }
    }
}