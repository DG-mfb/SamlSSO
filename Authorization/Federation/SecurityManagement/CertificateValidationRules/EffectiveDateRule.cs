using System;
using Kernel.Cryptography.Validation;
using Kernel.Logging;

namespace SecurityManagement.CertificateValidationRules
{
    internal class EffectiveDateRule : CertificateValidationRule
    {
        private readonly ILogProvider _logProvider;
        public EffectiveDateRule(ILogProvider logProvider)
        {
            this._logProvider = logProvider;
        }
        protected override void Internal(CertificateValidationContext context)
        {
            this._logProvider.LogMessage(String.Format("Validating effective date rule for context subject: {0}", context.Certificate.Subject));
            var certificate = context.Certificate;
            var effectiveDateString = certificate.GetEffectiveDateString();

            DateTime date;
            DateTime.TryParse(effectiveDateString, out date);
            if (date > DateTime.Now)
            {
                this._logProvider.LogMessage(String.Format("Certificate has effective date in the future: {0}", date));
                throw new InvalidOperationException("Certificate effective date.");
            }
            this._logProvider.LogMessage(String.Format("Certificate has effective date: {0}", date));
        }
    }
}