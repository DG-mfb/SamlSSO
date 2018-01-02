using Kernel.Federation.FederationPartner;

namespace Kernel.Federation.Protocols
{
    public interface ISamlRequestClauseBuilder<TRequest, TConfiguration> where TConfiguration : RequestConfiguration
    {
        void Build(TRequest request, TConfiguration configuration);
    }
}