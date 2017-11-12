namespace Kernel.Federation.FederationPartner
{
    public interface IDiscoveryService<TId>
    {
        TId ResolveParnerId(object context);
    }
}