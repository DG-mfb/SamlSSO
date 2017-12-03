using Kernel.DependancyResolver;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using WebApi.Claims;
using WebApi.CustomCofiguration;
using WebApi.Token;

namespace WebApi.App_Start
{
    internal class DIRegistration
    {
        public static void Register(IDependencyResolver resolver)
        {
            resolver.RegisterType<RelayStateCustomAppender>(Lifetime.Transient);
            resolver.RegisterType<CustomUserClaimsProvider>(Lifetime.Transient);
            resolver.RegisterType<SSOAuthenticationTokenProvider>(Lifetime.Transient);
            resolver.RegisterType<SSOAuthorizationServerProvider>(Lifetime.Transient);
            resolver.RegisterFactory<ISecureDataFormat<AuthenticationTicket>>(() =>
            {
                var protector = new DpapiDataProtectionProvider("SSO server").Create("token");
                var dataFormat = new TicketDataFormat(protector);
                return dataFormat;

            }, Lifetime.Singleton);
        }
    }
}