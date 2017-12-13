﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.MetaData;

namespace InlineMetadataContextProvider
{
    internal class FederationPartyContextBuilder : IAssertionPartyContextBuilder, IConfigurationManager<FederationPartyConfiguration>
    {
        public FederationPartyContextBuilder()
        {
           
        }

        public Task<FederationPartyConfiguration> GetConfigurationAsync(string federationPartyId, CancellationToken cancel)
        {
            var configuration = this.BuildContext(federationPartyId);
            return Task.FromResult(configuration);
        }

        public void RequestRefresh(string federationPartyId)
        {
            throw new NotImplementedException();
        }

        public FederationPartyConfiguration BuildContext(string federationPartyId, string metadataPath)
        {
            var metadataBuilder = new InlineMetadataContextBuilder();
            var metadataContext = metadataBuilder.BuildContext(new MetadataGenerateRequest(MetadataType.SP, federationPartyId));
            var configuration = new FederationPartyConfiguration(federationPartyId, metadataPath)
            {
                MetadataContext = metadataContext,
            };
            return configuration;
        }

        public FederationPartyConfiguration BuildContext(string federationPartyId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}