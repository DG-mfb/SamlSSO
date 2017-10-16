using System;

namespace Kernel.Security.Validation
{
    [Flags]
    public enum ValidationScope
    {
        Certificate = 1,
        BackchannelCertificate= 2
    }
}