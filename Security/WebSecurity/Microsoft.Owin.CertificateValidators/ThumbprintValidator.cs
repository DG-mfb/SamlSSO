using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kernel.Security.Validation;
using Microsoft.Owin.Security;

namespace Microsoft.Owin.CertificateValidators
{
    internal class ThumbprintValidator : CertificateThumbprintValidator, IPinningSertificateValidator
    {
        public ThumbprintValidator(IEnumerable<string> validThumbprints)
            : base(validThumbprints)
        {
        }

        public async Task Validate(object sender, BackchannelCertificateValidationContext context, Func<object, BackchannelCertificateValidationContext, Task> next)
        {
            var isValid = base.Validate(sender, context.Certificate, context.Chain, context.SslPolicyErrors);
            if (isValid)
                context.Validated();
            else
                await next(sender, context);
        }
    }
}