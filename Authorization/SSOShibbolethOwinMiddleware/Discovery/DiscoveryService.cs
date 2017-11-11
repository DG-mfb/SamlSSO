using System;
using Kernel.Federation.FederationPartner;
using Microsoft.Owin;

namespace SSOOwinMiddleware.Discovery
{
    public class DiscoveryService : IDiscoveryService<IOwinContext, string>
    {
        public Func<IOwinContext, string> Factory { private get; set; }

        public string ResolveParnerId(IOwinContext context)
        {
            if (this.Factory != null)
                return this.Factory(context);

            return FederationPartyIdentifierHelper.GetFederationPartyIdFromRequestOrDefault(context);
        }
    }
}