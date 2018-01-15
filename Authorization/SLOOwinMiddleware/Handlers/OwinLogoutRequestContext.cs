using System;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Request;
using Microsoft.Owin;

namespace SLOOwinMiddleware.Handlers
{
    internal class OwinLogoutRequestContext : LogoutRequestContext
    {
        public OwinLogoutRequestContext(IOwinContext context, Uri destination, Uri origin, FederationPartyConfiguration federationPartyContext, SamlLogoutContext samlLogoutContext)
             : base(destination, origin, federationPartyContext, samlLogoutContext)
        {
        }
    }
}