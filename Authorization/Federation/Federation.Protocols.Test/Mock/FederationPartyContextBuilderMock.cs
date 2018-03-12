using System;
using InlineMetadataContextProvider;
using Kernel.Federation.Constants;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.MetaData;

namespace Federation.Protocols.Test.Mock
{
    internal class FederationPartyContextBuilderMock : IAssertionPartyContextBuilder
    {
        private InlineMetadataContextBuilder _inlineMetadataContextBuilder = new InlineMetadataContextBuilder();

        public FederationPartyConfiguration BuildContext(string federationPartyId)
        {
            return this.BuildContext(federationPartyId, NameIdentifierFormats.Unspecified);
        }

        public FederationPartyConfiguration BuildContext(string federationPartyId, ushort assertionIndexEndpoint)
        {
            return this.BuildContext(federationPartyId, NameIdentifierFormats.Unspecified, new ScopingConfiguration(), assertionIndexEndpoint);
        }

        public FederationPartyConfiguration BuildContext(string federationPartyId, ScopingConfiguration scopingConfiguration)
        {
            return this.BuildContext(federationPartyId, NameIdentifierFormats.Unspecified, scopingConfiguration, 0);
        }

        public FederationPartyConfiguration BuildContext(string federationPartyId, RequestedAuthnContextConfiguration requestedAuthnContextConfiguration)
        {
            return this.BuildContext(federationPartyId, NameIdentifierFormats.Unspecified, new ScopingConfiguration(), requestedAuthnContextConfiguration, 0);
        }

        public FederationPartyConfiguration BuildContext(string federationPartyId, string defaultNameIdFormat)
        {
            return this.BuildContext(federationPartyId, defaultNameIdFormat, new ScopingConfiguration(), 0);
        }

        public FederationPartyConfiguration BuildContext(string federationPartyId, string defaultNameIdFormat, ScopingConfiguration scopingConfiguration, ushort assertionIndexEndpoint)
        {
            var requestedAuthnContextConfiguration = this.BuildRequestedAuthnContextConfiguration();
            return this.BuildContext(federationPartyId, defaultNameIdFormat, scopingConfiguration, requestedAuthnContextConfiguration, assertionIndexEndpoint);
        }

        public FederationPartyConfiguration BuildContext(string federationPartyId, string defaultNameIdFormat, ScopingConfiguration scopingConfiguration, RequestedAuthnContextConfiguration requestedAuthnContextConfiguration, ushort assertionIndexEndpoint)
        {
            var nameIdconfiguration = new DefaultNameId(new Uri(defaultNameIdFormat));
            var federationPartyAuthnRequestConfiguration = new FederationPartyAuthnRequestConfiguration(requestedAuthnContextConfiguration, nameIdconfiguration, scopingConfiguration);
            federationPartyAuthnRequestConfiguration.AssertionIndexEndpoint = assertionIndexEndpoint;
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