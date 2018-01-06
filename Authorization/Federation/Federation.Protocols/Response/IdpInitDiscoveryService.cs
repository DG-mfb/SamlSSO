using System;
using Kernel.Federation.FederationPartner;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class IdpInitDiscoveryService : IDiscoveryService<SamlResponseContext, string>
    {
        private readonly IAssertionPartyContextBuilder _fedeartionPartyContextBuilder;
        public Func<SamlResponseContext, string> Factory { private get; set; }
        public IdpInitDiscoveryService(IAssertionPartyContextBuilder fedeartionPartyContextBuilder)
        {
            this._fedeartionPartyContextBuilder = fedeartionPartyContextBuilder;
        }
        public string ResolveParnerId(SamlResponseContext context)
        {
            var conf = this._fedeartionPartyContextBuilder.BuildContext(context.StatusResponse.Issuer.Value);
            return conf != null ? conf.FederationPartyId : null;
        }

        public string ResolveParnerId(object context)
        {
            throw new NotImplementedException();
        }
    }
}