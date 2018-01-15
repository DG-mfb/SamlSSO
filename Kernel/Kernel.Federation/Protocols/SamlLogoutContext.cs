using System;
using System.IdentityModel.Tokens;

namespace Kernel.Federation.Protocols
{
    public class SamlLogoutContext
    {
        public SamlLogoutContext(Uri reason, Saml2NameIdentifier nameId, params string[] sessionIndex)
        {
            this.Reason = reason;
            this.NameId = nameId;
            this.SessionIndex = sessionIndex;
        }
        public Uri Reason { get; }
        public Saml2NameIdentifier NameId { get; }
        public string[] SessionIndex { get; }
    }
}