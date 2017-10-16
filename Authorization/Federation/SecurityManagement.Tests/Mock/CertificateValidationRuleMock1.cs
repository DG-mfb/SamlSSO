using System;
using System.Threading.Tasks;
using Kernel.Security.Validation;

namespace SecurityManagement.Tests.Mock
{
    internal class CertificateValidationRuleMock1 : ICertificateValidationRule
    {
        public Task Validate(CertificateValidationContext context, Func<CertificateValidationContext, Task> next)
        {
            return next(context);
        }
    }
}