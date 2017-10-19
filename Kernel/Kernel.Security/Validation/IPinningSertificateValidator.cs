using System;
using System.Threading.Tasks;

namespace Kernel.Security.Validation
{
    public interface IPinningSertificateValidator
    {
        Task Validate(object sender, BackchannelCertificateValidationContext context, Func<object, BackchannelCertificateValidationContext, Task> next);
    }
}