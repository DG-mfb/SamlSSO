using System;

namespace Kernel.Security.Validation
{
    public interface ICertificateValidationConfigurationProvider : IDisposable
    {
        CertificateValidationConfiguration GetConfiguration(string federationPartyId);
    }
}