using System;
using Kernel.Federation.FederationPartner;

namespace Kernel.Federation.Protocols.Request
{
    public class LogoutRequestContext : RequestContext
    {
        public LogoutRequestContext(Uri destination, Uri origin, FederationPartyConfiguration federationPartyContext)
            : base(destination, origin, federationPartyContext)
        {
            this.RequestId = String.Format("{0}_{1}", federationPartyContext.MetadataContext.EntityDesriptorConfiguration.Id, Guid.NewGuid().ToString());
        }

        public string RequestId { get; }
    }
}