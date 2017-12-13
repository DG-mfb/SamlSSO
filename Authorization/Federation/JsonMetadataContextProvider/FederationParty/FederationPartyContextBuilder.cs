using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kernel.Cache;
using Kernel.Federation.FederationPartner;
using Serialisation.JSON;

namespace JsonMetadataContextProvider
{
    internal class FederationPartyContextBuilder : IAssertionPartyContextBuilder, IConfigurationManager<FederationPartyConfiguration>
    {
        private readonly IJsonSerialiser  _serialiser;
        private readonly ICacheProvider _cacheProvider;
        private string _source;
        public FederationPartyContextBuilder(IJsonSerialiser serialiser, ICacheProvider cacheProvider, Func<string> source)
        {
            this._serialiser = serialiser;
            this._cacheProvider = cacheProvider;
            this._source = source();
        }

        public Task<FederationPartyConfiguration> GetConfigurationAsync(string federationPartyId, CancellationToken cancel)
        {
            var configuration = this.BuildContext(federationPartyId);
            return Task.FromResult(configuration);
        }

        public void RequestRefresh(string federationPartyId)
        {
            if (String.IsNullOrWhiteSpace(federationPartyId))
                throw new ArgumentNullException("federationPartyId");
            this._cacheProvider.Delete(federationPartyId);
        }

        public FederationPartyConfiguration BuildContext(string federationPartyId)
        {
            if (this._cacheProvider.Contains(federationPartyId))
                return this._cacheProvider.Get<FederationPartyConfiguration>(federationPartyId);

            var configurations = this._serialiser.Deserialize<IEnumerable<FederationPartyConfiguration>>(this._source);
            var configuration = configurations.First(x => x.FederationPartyId == federationPartyId);
            this._cacheProvider.Put(federationPartyId, configuration);
            return configuration;

        }

        public void Dispose()
        {
        }
    }
}