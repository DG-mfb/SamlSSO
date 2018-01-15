namespace Kernel.Federation.Protocols
{
    public interface ISamlLogoutContextResolver<T>
    {
        SamlLogoutContext ResolveLogoutContext(T request);
    }
}