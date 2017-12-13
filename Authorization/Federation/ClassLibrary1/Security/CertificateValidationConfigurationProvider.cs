using System;
using System.Linq.Expressions;
using Kernel.Federation.FederationPartner;
using Kernel.Security.Configuration;
using Kernel.Security.Validation;

namespace InlineMetadataContextProvider.Security
{
    internal class CertificateValidationConfigurationProvider : ICertificateValidationConfigurationProvider
    {
        public CertificateValidationConfigurationProvider()
        {
        }

        public BackchannelConfiguration GeBackchannelConfiguration(string federationPartyId)
        {
            return this.GeBackchannelConfiguration(x => x.FederationPartyId == federationPartyId, federationPartyId);
        }

        public BackchannelConfiguration GeBackchannelConfiguration(Uri partyUri)
        {
            return this.GeBackchannelConfiguration(x => x.MetadataAddress == partyUri.AbsoluteUri, partyUri.AbsoluteUri);
        }

        public BackchannelConfiguration GeBackchannelConfiguration(Expression<Func<FederationPartyConfiguration, bool>> predicate, string keyPrefex)
        {
           
            var configuration = new BackchannelConfiguration
            {
                UsePinningValidation = false,
                //BackchannelValidatorResolver = new Kernel.Data.TypeDescriptor("Microsoft.Owin.CertificateValidators.CertificateValidatorResolver, Microsoft.Owin.CertificateValidators, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null")
            };
            
            return configuration;
        }

        public CertificateValidationConfiguration GetConfiguration(string federationPartyId)
        {
            var configuration = new CertificateValidationConfiguration
            {
                X509CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom
            };
           
            return configuration;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}