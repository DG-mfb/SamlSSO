using System;
using System.Collections.Generic;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.Protocols.Request;
using Microsoft.Owin;

namespace SSOOwinMiddleware
{
    public class OwinAuthnRequestContext : AuthnRequestContext
    {
        public IOwinContext Context { get; }
        public OwinAuthnRequestContext(IOwinContext context, Uri destination, Uri origin, FederationPartyConfiguration federationPartyContext, ICollection<Uri> supportedNameIdentifierFormats) : base(destination, origin, federationPartyContext, supportedNameIdentifierFormats)
        {
            this.Context = context;
        }
    }
}
