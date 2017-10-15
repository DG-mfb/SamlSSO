using InlineMetadataContextProvider;
using Kernel.Federation.MetaData;
using Kernel.Federation.FederationPartner;
using Shared.Federtion.Constants;
using System;

namespace Federation.Protocols.Test.Mock
{
    internal class FederationPartyContextBuilderMock : IFederationPartyContextBuilder
    {
        private InlineMetadataContextBuilder _inlineMetadataContextBuilder = new InlineMetadataContextBuilder();

        public FederationPartyConfiguration BuildContext(string federationPartyId)
        {
            return this.BuildContext(federationPartyId, NameIdentifierFormats.Unspecified);
        }

        public FederationPartyConfiguration BuildContext(string federationPartyId, ScopingConfiguration scopingConfiguration)
        {
            return this.BuildContext(federationPartyId, NameIdentifierFormats.Unspecified, scopingConfiguration);
        }

        public FederationPartyConfiguration BuildContext(string federationPartyId, string defaultNameIdFormat)
        {
            return this.BuildContext(federationPartyId, defaultNameIdFormat, new ScopingConfiguration());
        }

        public FederationPartyConfiguration BuildContext(string federationPartyId, string defaultNameIdFormat, ScopingConfiguration scopingConfiguration)
        {
            var requestedAuthnContextConfiguration = this.BuildRequestedAuthnContextConfiguration();
            var nameIdconfiguration = new DefaultNameId(new Uri(defaultNameIdFormat));
            var federationPartyAuthnRequestConfiguration = new FederationPartyAuthnRequestConfiguration(requestedAuthnContextConfiguration, nameIdconfiguration, scopingConfiguration);

            return new FederationPartyConfiguration("local", "https://dg-mfb/idp/shibboleth")
            {
                MetadataContext = this._inlineMetadataContextBuilder.BuildContext(new MetadataGenerateRequest(MetadataType.SP, "local")),
                FederationPartyAuthnRequestConfiguration = federationPartyAuthnRequestConfiguration
            };
        }

        private RequestedAuthnContextConfiguration BuildRequestedAuthnContextConfiguration()
        {
            return new RequestedAuthnContextConfiguration("Exact")
            {
            };
        }
        public void Dispose()
        {
        }
    }
}