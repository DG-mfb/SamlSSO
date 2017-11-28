using System;
using System.Collections.Generic;
using Kernel.Federation.FederationPartner;

namespace Kernel.Federation.Protocols
{
    public class AuthnRequestContext
    {
        public AuthnRequestContext(Uri destination, Uri origin, FederationPartyConfiguration federationPartyContext, ICollection<Uri> supportedNameIdentifierFormats)
        {
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (federationPartyContext == null)
                throw new ArgumentNullException("federationPartyContext");
            if (supportedNameIdentifierFormats == null)
                throw new ArgumentNullException("federationPartyContext");
            if (origin == null)
                throw new ArgumentNullException("origin");
            this.Origin = origin;
            this.SupportedNameIdentifierFormats = supportedNameIdentifierFormats;
            this.FederationPartyContext = federationPartyContext;
            this.Destination = destination;
            this.RelyingState = new Dictionary<string, object>();
            this.RequestId = String.Format("{0}_{1}", federationPartyContext.MetadataContext.EntityDesriptorConfiguration.Id, Guid.NewGuid().ToString());
        }
        public Uri Origin { get; }
        public string RequestId { get; }
        public IDictionary<string, object> RelyingState { get; }
        public Uri Destination { get; }
        public ICollection<Uri> SupportedNameIdentifierFormats { get; }
        public FederationPartyConfiguration FederationPartyContext { get; }
    }
}