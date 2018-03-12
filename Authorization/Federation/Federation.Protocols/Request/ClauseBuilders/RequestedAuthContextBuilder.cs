using System;
using System.Linq;
using Kernel.DependancyResolver;
using Kernel.Federation.FederationPartner;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request.ClauseBuilders
{
    internal class RequestedAuthContextBuilder : AutnRequestClauseBuilder
    {
        public RequestedAuthContextBuilder(IDependencyResolver dependencyResolver) : base(dependencyResolver)
        {
        }

        protected override void BuildInternal(AuthnRequest request, AuthnRequestConfiguration configuration)
        {
            if (configuration.RequestedAuthnContextConfiguration == null)
                return;
            var comparison = (AuthnContextComparisonType)Enum.Parse(typeof(AuthnContextComparisonType), configuration.RequestedAuthnContextConfiguration.Comparision);
            request.RequestedAuthnContext = new RequestedAuthnContext
            {
                Comparison = comparison,
                ItemsElementName = configuration.RequestedAuthnContextConfiguration.RequestedAuthnContexts.Select(x => (AuthnContextType)Enum.Parse(typeof(AuthnContextType), x.AuthnContextType)).ToArray(),
                Items = configuration.RequestedAuthnContextConfiguration.RequestedAuthnContexts.Select(x => x.AuthnContextUri.AbsoluteUri).ToArray()
            };
        }
    }
}