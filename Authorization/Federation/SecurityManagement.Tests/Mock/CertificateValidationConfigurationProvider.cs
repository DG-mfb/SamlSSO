using System;
using Kernel.Security.Configuration;
using Kernel.Security.Validation;

namespace SecurityManagement.Tests.Mock
{
    internal class CertificateValidationConfigurationProvider : ICertificateValidationConfigurationProvider
    {
        Func<CertificateValidationConfiguration> _func;
        Func<BackchannelConfiguration> _backChannelfunc;
        public CertificateValidationConfigurationProvider(Func<CertificateValidationConfiguration> func)
        {
            this._func = func;
        }

        public CertificateValidationConfigurationProvider(Func<BackchannelConfiguration> func)
        {
            this._backChannelfunc = func;
        }
        public CertificateValidationConfiguration GetConfiguration(string federationPartyId)
        {
            return this._func();
        }
        public BackchannelConfiguration GeBackchannelConfiguration(string federationPartyId)
        {
            return this._backChannelfunc();
        }
        public void Dispose()
        {
        }
    }
}
