using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.Cache;
using Kernel.Security.Configuration;
using Kernel.Security.Validation;
using Serialisation.JSON;

namespace JsonMetadataContextProvider.Security
{
    internal class CertificateValidationConfigurationProvider : ICertificateValidationConfigurationProvider
    {
        private const string PinsKey = "{0}_backchannel_key";
        private const string CertKey = "{0}_cert_key";
        private readonly IJsonSerialiser _serialiser;
        private readonly ICacheProvider _cacheProvider;
        private Func<Type, string> _source;

        public CertificateValidationConfigurationProvider(IJsonSerialiser serialiser, ICacheProvider cacheProvider, Func<Type, string> source)
        {
            this._serialiser = serialiser;
            this._source = source;
            this._cacheProvider = cacheProvider;
        }

        public BackchannelConfiguration GeBackchannelConfiguration(string federationPartyId)
        {
            return this.GeBackchannelConfiguration(x => x.id == federationPartyId, federationPartyId);
        }

        public BackchannelConfiguration GeBackchannelConfiguration(Uri partyUri)
        {
            return this.GeBackchannelConfiguration(x => x.metadataAddress == partyUri.AbsoluteUri, partyUri.AbsoluteUri);
        }

        public BackchannelConfiguration GeBackchannelConfiguration(Func<dynamic, bool> predicate, string keyPrefex)
        {
            var key = String.Format(CertificateValidationConfigurationProvider.PinsKey, keyPrefex);
            if (this._cacheProvider.Contains(key))
                return this._cacheProvider.Get<BackchannelConfiguration>(key);

            var serialised = this._source(this.GetType());
            var deserialised = this._serialiser.Deserialize<IEnumerable<object>>(serialised);
            var result = deserialised.Where(x => ((dynamic)x).configuration.GetType() == typeof(BackchannelConfiguration))
                .Where(predicate)
                .SingleOrDefault();
            if (result == null)
                throw new InvalidOperationException("No backchannel configuration found.");
            var configuration = result.configuration;
            this._cacheProvider.Put(key, configuration);
            
            return configuration;
        }

        public CertificateValidationConfiguration GetConfiguration(string federationPartyId)
        {
            var key = String.Format(CertificateValidationConfigurationProvider.CertKey, federationPartyId);
            if (this._cacheProvider.Contains(key))
                return this._cacheProvider.Get<CertificateValidationConfiguration>(key);

            var serialised = this._source(this.GetType());
            var deserialised = this._serialiser.Deserialize<IEnumerable<object>>(serialised);
            var result = deserialised.Where(x => ((dynamic)x).id == federationPartyId && ((dynamic)x).configuration.GetType() == typeof(CertificateValidationConfiguration))
                .SingleOrDefault();
            if (result == null)
                throw new InvalidOperationException("No backchannel configuration found.");
            var configuration = ((dynamic)result).configuration;
            this._cacheProvider.Put(key, configuration);

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