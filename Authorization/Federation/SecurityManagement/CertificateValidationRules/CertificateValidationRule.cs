using System;
using System.Threading.Tasks;
using Kernel.Logging;
using Kernel.Security.Validation;

namespace SecurityManagement.CertificateValidationRules
{
    internal abstract class CertificateValidationRule : ICertificateValidationRule
    {
        protected readonly ILogProvider _logProvider;
        public CertificateValidationRule(ILogProvider logProvider)
        {
            this._logProvider = logProvider;
        }
        public Task Validate(CertificateValidationContext context, Func<CertificateValidationContext, Task> next)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (context.Certificate == null)
                throw new ArgumentNullException("certificate");

            this.Internal(context);
            return next(context);
        }

        protected abstract void Internal(CertificateValidationContext context);
    }
}