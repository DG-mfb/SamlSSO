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
            if (context == null)
                throw new ArgumentNullException("context");

            if (this.Factory != null)
                return this.Factory(context);

            return FederationPartyIdentifierHelper.GetFederationPartyIdFromRequestOrDefault(context);
        }

        string IDiscoveryService<string>.ResolveParnerId(object context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if(context is IOwinContext)
                return this.ResolveParnerId((IOwinContext)context);
            throw new InvalidOperationException(String.Format("Expected context type of: {0}, but it was: {1}", typeof(IOwinContext).Name, context.GetType().Name));
        }
    }
}