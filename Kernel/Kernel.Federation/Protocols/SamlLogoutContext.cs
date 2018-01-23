using System;
using System.IdentityModel.Tokens;

namespace Kernel.Federation.Protocols
{
    public class SamlLogoutContext
    {
        public SamlLogoutContext(Uri reason, Saml2NameIdentifier nameId, string federationPartyId, params string[] sessionIndex)
        {
            this.Reason = reason;
            this.NameId = nameId;
            this.FederationPartyId = federationPartyId;
            this.SessionIndex = sessionIndex;
        }
        public Uri Reason { get; }
        public Saml2NameIdentifier NameId { get; }
        public string[] SessionIndex { get; }
        public string FederationPartyId { get; }
    }
}