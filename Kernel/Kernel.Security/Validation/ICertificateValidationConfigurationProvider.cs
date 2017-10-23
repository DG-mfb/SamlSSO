using System;
using Kernel.Security.Configuration;

namespace Kernel.Security.Validation
{
    public interface ICertificateValidationConfigurationProvider : IDisposable
    {
        CertificateValidationConfiguration GetConfiguration(string federationPartyId);
        BackchannelConfiguration GeBackchannelConfiguration(string federationPartyId);
    }
}