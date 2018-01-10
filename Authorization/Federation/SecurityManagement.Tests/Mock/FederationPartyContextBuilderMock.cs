using System;
using InlineMetadataContextProvider;
using Kernel.Federation.Constants;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.MetaData;

namespace SecurityManagement.Tests.Mock
{
    internal class FederationPartyContextBuilderMock : IAssertionPartyContextBuilder
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

        public FederationPartyConfiguration BuildContext(string federationPartyId, RequestedAuthnContextConfiguration requestedAuthnContextConfiguration)
        {
            return this.BuildContext(federationPartyId, NameIdentifierFormats.Unspecified, new ScopingConfiguration(), requestedAuthnContextConfiguration);
        }

        public FederationPartyConfiguration BuildContext(string federationPartyId, string defaultNameIdFormat)
        {
            return this.BuildContext(federationPartyId, defaultNameIdFormat, new ScopingConfiguration());
        }

        public FederationPartyConfiguration BuildContext(string federationPartyId, string defaultNameIdFormat, ScopingConfiguration scopingConfiguration)
        {
            var requestedAuthnContextConfiguration = this.BuildRequestedAuthnContextConfiguration();
            return this.BuildContext(federationPartyId, defaultNameIdFormat, scopingConfiguration, requestedAuthnContextConfiguration);
        }

        public FederationPartyConfiguration BuildContext(string federationPartyId, string defaultNameIdFormat, ScopingConfiguration scopingConfiguration, RequestedAuthnContextConfiguration requestedAuthnContextConfiguration)
        {
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