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
            this.RequestId = String.Format("{0}_{1}", federationPartyContext.MetadataContext.EntityDesriptorConfiguration.Id, Guid.NewGuid().ToString());
        }
        public Uri Reason { get; }
        public string RequestId { get; }
    }
}