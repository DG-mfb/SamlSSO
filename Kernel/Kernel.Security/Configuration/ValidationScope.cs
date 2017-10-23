using System;

namespace Kernel.Security.Configuration
{
    [Flags]
    public enum ValidationScope
    {
        Certificate = 1,
        BackchannelCertificate= 2
    }
}