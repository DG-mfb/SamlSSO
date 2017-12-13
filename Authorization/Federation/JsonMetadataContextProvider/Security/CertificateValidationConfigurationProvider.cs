using System;
using System.Linq;
using System.Linq.Expressions;
using Kernel.Cache;
using Kernel.Federation.FederationPartner;
using Kernel.Security.Configuration;
using Kernel.Security.Validation;

namespace JsonMetadataContextProvider.Security
{
    internal class CertificateValidationConfigurationProvider : ICertificateValidationConfigurationProvider
    {
        private const string PinsKey = "{0}_backchannel_key";
        
        private readonly ICacheProvider _cacheProvider;

        public CertificateValidationConfigurationProvider(ICacheProvider cacheProvider)
        {
           
            this._cacheProvider = cacheProvider;
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
            var key = String.Format(CertificateValidationConfigurationProvider.PinsKey, keyPrefex);
            if (this._cacheProvider.Contains(key))
                return this._cacheProvider.Get<BackchannelConfiguration>(key);
            
            var configuration = new BackchannelConfiguration
            {
                UsePinningValidation = false,
                //BackchannelValidatorResolver = new Kernel.Data.TypeDescriptor(settings.SecuritySettings.PinnedTypeValidator)
            };

            //configuration.Pins = settings.Pins.GroupBy(k => k.PinType, v => v.Value)
            //    .ToDictionary(k => k.Key, v => v.Select(r => r));
            //this._cacheProvider.Put(key, configuration);
            return configuration;
        }

        public CertificateValidationConfiguration GetConfiguration(string federationPartyId)
        {
           

            var configuration = new CertificateValidationConfiguration
            {
                X509CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None
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