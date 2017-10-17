using System;
using Kernel.Logging;
using Kernel.Security.Validation;

namespace SecurityManagement.CertificateValidationRules
{
    internal class ExpirationDateRule : CertificateValidationRule
    {
        public ExpirationDateRule(ILogProvider logProvider) : base(logProvider)
        {
        }

        protected override void Internal(CertificateValidationContext context)
        {
            base._logProvider.LogMessage(String.Format("Validating expiration date rule for context subject: {0}", context.Certificate.Subject));
            var certificate = context.Certificate;
            var expirationDateString = certificate.GetExpirationDateString();
            
            DateTimeOffset date;
            DateTimeOffset.TryParse(expirationDateString, out date);
            if (date < DateTimeOffset.Now)
            {
                base._logProvider.LogMessage(String.Format("Certificate has expired on: {0}", date));
                throw new InvalidOperationException("Certificate has expired");
            }
            base._logProvider.LogMessage(String.Format("Certificate is valid until: {0}", date));
        }
    }
}