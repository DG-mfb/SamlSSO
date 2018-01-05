using System;
using System.Collections.Generic;
using Kernel.Federation.FederationPartner;

namespace Kernel.Federation.Protocols.Request
{
    public class AuthnRequestContext : RequestContext
    {
        public AuthnRequestContext(Uri destination, Uri origin, FederationPartyConfiguration federationPartyContext, ICollection<Uri> supportedNameIdentifierFormats)
            : base(destination, origin, federationPartyContext)
        {
            if (supportedNameIdentifierFormats == null)
                throw new ArgumentNullException("supportedNameIdentifierFormats");
          
            this.SupportedNameIdentifierFormats = supportedNameIdentifierFormats;
        }
        
        public ICollection<Uri> SupportedNameIdentifierFormats { get; }
    }
}