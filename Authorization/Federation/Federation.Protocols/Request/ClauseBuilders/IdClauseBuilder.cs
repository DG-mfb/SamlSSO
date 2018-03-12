using Kernel.DependancyResolver;
using Kernel.Federation.FederationPartner;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request.ClauseBuilders
{
    internal class IdClauseBuilder : AutnRequestClauseBuilder
    {
        public IdClauseBuilder(IDependencyResolver dependencyResolver) : base(dependencyResolver)
        {
        }

        protected override void BuildInternal(AuthnRequest request, AuthnRequestConfiguration configuration)
        {
            request.Id = configuration.RequestId;
        }
    }
}