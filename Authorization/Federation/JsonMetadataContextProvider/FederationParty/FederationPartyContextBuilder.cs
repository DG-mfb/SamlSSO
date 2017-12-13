using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
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
            //using (var reader = new StreamReader(this._path))
            {
                //var content = reader.ReadToEnd();
                var configuration = this._serialiser.Deserialize<IEnumerable<FederationPartyConfiguration>>(this._source);
                return configuration.First(x => x.FederationPartyId == federationPartyId);
            }
        }

        public void Dispose()
        {
        }
    }
}