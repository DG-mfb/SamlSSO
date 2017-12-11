using System;
using Kernel.Federation.FederationPartner;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class IdpInitDiscoveryService : IDiscoveryService<ResponseStatus, string>
    {
        private readonly IAssertionPartyContextBuilder _fedeartionPartyContextBuilder;
        public Func<ResponseStatus, string> Factory { private get; set; }
        public IdpInitDiscoveryService(IAssertionPartyContextBuilder fedeartionPartyContextBuilder)
        {
            this._fedeartionPartyContextBuilder = fedeartionPartyContextBuilder;
        }
        public string ResolveParnerId(ResponseStatus context)
        {
            var conf = this._fedeartionPartyContextBuilder.BuildContext(context.Issuer);
            return conf != null ? conf.FederationPartyId : null;
        }

        public string ResolveParnerId(object context)
        {
            throw new NotImplementedException();
        }
    }
}