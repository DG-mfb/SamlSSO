using Kernel.DependancyResolver;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.Protocols;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request.ClauseBuilders
{
    internal abstract class AutnRequestClauseBuilder : ISamlRequestClauseBuilder<AuthnRequest, AuthnRequestConfiguration>
    {
        protected IDependencyResolver DependencyResolver;
        public AutnRequestClauseBuilder(IDependencyResolver dependencyResolver)
        {
            this.DependencyResolver = dependencyResolver;
        }
        public void Build(AuthnRequest request, AuthnRequestConfiguration configuration)
        {
            this.BuildInternal(request, configuration);
        }

        protected abstract void BuildInternal(AuthnRequest request, AuthnRequestConfiguration configuration);
    }
}