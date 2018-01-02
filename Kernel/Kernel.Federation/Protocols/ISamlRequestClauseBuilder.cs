using Kernel.Federation.FederationPartner;

namespace Kernel.Federation.Protocols
{
    public interface ISamlRequestClauseBuilder<TRequest>
    {
        void Build(TRequest request, AuthnRequestConfiguration configuration);
    }
}