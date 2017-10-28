using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Kernel.Security.Validation;

namespace Microsoft.Owin.CertificateValidators
{
    internal class LocalThumbprintValidator : IPinningSertificateValidator
    {
        IEnumerable<string> _validThumbprints;
        public LocalThumbprintValidator(IEnumerable<string> validThumbprints)
        {
            this._validThumbprints = validThumbprints;
        }

        public async Task Validate(object sender, BackchannelCertificateValidationContext context, Func<object, BackchannelCertificateValidationContext, Task> next)
        {
#if(DEBUG)
            var httpMessage = sender as HttpWebRequest;
            if (httpMessage != null && httpMessage.Host == "dg-mfb")
            {
                foreach (X509ChainElement chainElement in context.Chain.ChainElements)
                {
                    string thumbprint = chainElement.Certificate.Thumbprint;
                    if (thumbprint != null && this._validThumbprints.Contains(thumbprint))
                    {
                        context.Validated();
                        break;
                    }
                }
            }
            else
            {
                await next(sender, context);
            }
#endif
        }
    }
}