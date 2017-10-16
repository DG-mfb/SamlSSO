using System.Collections.Generic;

namespace Kernel.Security.Validation
{
    public interface ICertificateValidatorResolver
    {
        IEnumerable<TValidator> Resolve<TValidator>();
    }
}