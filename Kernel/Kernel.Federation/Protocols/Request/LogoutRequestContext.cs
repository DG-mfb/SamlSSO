using System;
using Kernel.Federation.FederationPartner;

namespace Kernel.Federation.Protocols.Request
{
    public class LogoutRequestContext : RequestContext
    {
        public LogoutRequestContext(Uri destination, Uri origin, FederationPartyConfiguration federationPartyContext, SamlLogoutContext samlLogoutContext)
            : base(destination, origin, federationPartyContext)
        {
            this.SamlLogoutContext = samlLogoutContext;
        }
        public SamlLogoutContext SamlLogoutContext { get; }
    }
}