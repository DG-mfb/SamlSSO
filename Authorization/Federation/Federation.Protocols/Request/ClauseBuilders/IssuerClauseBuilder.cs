using Kernel.DependancyResolver;
using Kernel.Federation.Constants;
using Kernel.Federation.FederationPartner;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request.ClauseBuilders
{
    internal class IssuerClauseBuilder : AutnRequestClauseBuilder
    {
        public IssuerClauseBuilder(IDependencyResolver dependencyResolver) : base(dependencyResolver)
        {
        }

        protected override void BuildInternal(AuthnRequest request, AuthnRequestConfiguration configuration)
        {
            request.Issuer = new NameId
            {
                Value = configuration.EntityId,
                Format = NameIdentifierFormats.Entity
            };
        }
    }
}