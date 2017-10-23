using System;
using System.Collections.Generic;
using Kernel.Security.Configuration;

namespace Kernel.Security.Validation
{
    public interface ICertificateValidatorResolver
    {
        IEnumerable<TValidator> Resolve<TValidator>(Uri partnerId) where TValidator : class;
        IEnumerable<IPinningSertificateValidator> Resolve(BackchannelConfiguration configuration); 
        
    }
}