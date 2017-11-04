using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kernel.DependancyResolver;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.MetaData;
using Kernel.Federation.MetaData.Configuration;

namespace FederationIdentityProvider.RelyingPartyConfiguration
{
    internal class RelyingPartyContextBuilder : IRelyingPartyContextBuilder
    {
        private readonly IDependencyResolver _resolver;

        public RelyingPartyContextBuilder(IDependencyResolver resolver)
        {
            this._resolver = resolver;
        }
        public FederationPartyConfiguration BuildContext(string federationPartyId)
        {
            var contextBuilder = this._resolver.Resolve<IInlineMetadataContextBuilder>();
            var metadataRequest = new MetadataGenerateRequest(MetadataType.Idp, federationPartyId);
            var metadataContext = contextBuilder.BuildContext(metadataRequest);
            var context = new FederationPartyConfiguration(metadataRequest.FederationPartyId, "localhost");
            context.MetadataContext = metadataContext;
            return context;
        }

        public void Dispose()
        {
        }
    }
}