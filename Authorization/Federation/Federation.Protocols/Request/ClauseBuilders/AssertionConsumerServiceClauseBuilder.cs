using Kernel.DependancyResolver;
using Kernel.Federation.FederationPartner;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request.ClauseBuilders
{
    internal class AssertionConsumerServiceClauseBuilder : AutnRequestClauseBuilder
    {
        public AssertionConsumerServiceClauseBuilder(IDependencyResolver dependencyResolver) : base(dependencyResolver)
        {
        }

        protected override void BuildInternal(AuthnRequest request, AuthnRequestConfiguration configuration)
        {
            request.AssertionConsumerServiceIndex = configuration.AssertionConsumerServiceIndex;
        }
    }
}