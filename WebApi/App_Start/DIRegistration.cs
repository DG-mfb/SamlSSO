using Kernel.DependancyResolver;
using WebApi.Claims;
using WebApi.CustomCofiguration;

namespace WebApi.App_Start
{
    internal class DIRegistration
    {
        public static void Register(IDependencyResolver resolver)
        {
            resolver.RegisterType<RelayStateCustomAppender>(Lifetime.Transient);
            resolver.RegisterType<CustomUserClaimsProvider>(Lifetime.Transient);
        }
    }
}