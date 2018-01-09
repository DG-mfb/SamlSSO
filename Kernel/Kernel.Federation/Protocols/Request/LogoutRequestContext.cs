using System;
using Kernel.Federation.FederationPartner;

namespace Kernel.Federation.Protocols.Request
{
    public class LogoutRequestContext : RequestContext
    {
        public LogoutRequestContext(Uri destination, Uri origin, FederationPartyConfiguration federationPartyContext, Uri reason)
            : base(destination, origin, federationPartyContext)
        {
            this.Reason = reason;
        }
        public Uri Reason { get; }
    }
}