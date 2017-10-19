using System;
using System.Threading.Tasks;
using Kernel.Security.Validation;

namespace Microsoft.Owin.CertificateValidators
{
    internal abstract class PinningSertificateValidator : IPinningSertificateValidator
    {
        public Task Validate(object sender, BackchannelCertificateValidationContext context, Func<object, BackchannelCertificateValidationContext, Task> next)
        {
            throw new NotImplementedException();
        }
    }
}