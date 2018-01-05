using System;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.Protocols.Request;
using Microsoft.Owin;

namespace SLOOwinMiddleware.Handlers
{
    internal class OwinLogoutRequestContext : LogoutRequestContext
    {
        public OwinLogoutRequestContext(IOwinContext context, Uri destination, Uri origin, FederationPartyConfiguration federationPartyContext, Uri reason)
             :base(destination, origin, federationPartyContext, reason)
        {
        }
    }
}