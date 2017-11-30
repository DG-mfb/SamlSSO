using Kernel.DependancyResolver;
using WebApi.CustomCofiguration;

namespace WebApi.App_Start
{
    internal class DIRegistration
    {
        public static void Register(IDependencyResolver resolver)
        {
            resolver.RegisterType<RelayStateCustomConfigurator>(Lifetime.Transient);
        }
    }
}