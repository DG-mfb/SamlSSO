using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Kernel.Configuration;
using Kernel.Security.Validation;
using Kernel.Web;

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
            bool backchannelLocalValidatorEnabled;
            var found = AppSettingsConfigurationManager.TryGetSettingAndParse<bool>("backchannelLocalValidatorEnabled", false, out backchannelLocalValidatorEnabled);
            if (!backchannelLocalValidatorEnabled)
                return;
#if(DEBUG)
            var webRequest = sender as HttpWebRequest;
            if (webRequest == null)
                return;

            var isLocal = Utility.IsLocalIpAddress(webRequest.Host);
            if (webRequest != null && isLocal)
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