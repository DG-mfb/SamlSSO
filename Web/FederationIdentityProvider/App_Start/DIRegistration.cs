using FederationIdentityProvider.RelyingPartyConfiguration;
using Kernel.DependancyResolver;

namespace FederationIdentityProvider.App_Start
{
    internal class DIRegistration
    {
        public static void Register(IDependencyResolver resolver)
        {
            resolver.RegisterType<RelyingPartyContextBuilder>(Lifetime.Transient);
        }
    }
}