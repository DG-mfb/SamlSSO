using System;
using Kernel.Federation.FederationPartner;

namespace Kernel.Federation.Protocols
{
    public class LogoutRequestContext : RequestContext
    {
        public LogoutRequestContext(Uri destination, Uri origin, FederationPartyConfiguration federationPartyContext)
            : base(destination, origin, federationPartyContext)
        {
            
        }
    }
}