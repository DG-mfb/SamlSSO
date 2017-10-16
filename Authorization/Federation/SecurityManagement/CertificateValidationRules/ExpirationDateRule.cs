using System;
using Kernel.Cryptography.Validation;
using Kernel.Logging;

namespace SecurityManagement.CertificateValidationRules
{
    internal class ExpirationDateRule : CertificateValidationRule
    {
        private readonly ILogProvider _logProvider;
        public ExpirationDateRule(ILogProvider logProvider)
        {
            this._logProvider = logProvider;
        }

        protected override void Internal(CertificateValidationContext context)
        {
            this._logProvider.LogMessage(String.Format("Validating expiration date rule for context subject: {0}", context.Certificate.Subject));
            var certificate = context.Certificate;
            var expirationDateString = certificate.GetExpirationDateString();
            
            DateTime date;
            DateTime.TryParse(expirationDateString, out date);
            if (date < DateTime.Now)
            {
                this._logProvider.LogMessage(String.Format("Certificate has expired on: {0}", date));
                throw new InvalidOperationException("Certificate has expired");
            }
            this._logProvider.LogMessage(String.Format("Certificate is valid until: {0}", date));
        }
    }
}