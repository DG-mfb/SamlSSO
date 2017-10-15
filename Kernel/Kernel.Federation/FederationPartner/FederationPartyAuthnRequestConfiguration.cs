namespace Kernel.Federation.FederationPartner
{
    public class FederationPartyAuthnRequestConfiguration
    {
        public FederationPartyAuthnRequestConfiguration(RequestedAuthnContextConfiguration requestedAuthnContextConfiguration, DefaultNameId defaultNameId, ScopingConfiguration scopingConfiguration)
        {
            this.RequestedAuthnContextConfiguration = requestedAuthnContextConfiguration;
            this.DefaultNameId = defaultNameId;
            this.ScopingConfiguration = scopingConfiguration;
        }

        public bool IsPassive { get; set; }
        public bool ForceAuthn { get; set; }
        public string Version { get; set; }
        public RequestedAuthnContextConfiguration RequestedAuthnContextConfiguration { get; }
        public DefaultNameId DefaultNameId { get; }
        public ScopingConfiguration ScopingConfiguration { get; }
    }
}