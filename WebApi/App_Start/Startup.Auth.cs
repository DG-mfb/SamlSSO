using Kernel.Authorisation;
using Kernel.Federation.MetaData;
using Kernel.Initialisation;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SLOOwinMiddleware.Extensions;
using SSOOwinMiddleware.Extensions;
using System.Threading.Tasks;

namespace WebApi
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        
        public void ConfigureAuth(IAppBuilder app)
        {
            var cookieOption = new CookieAuthenticationOptions
            {
                AuthenticationType = "Federation",
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = c =>
                    {

                    },
                    OnException = e =>
                    {

                    },
                    OnResponseSignedIn = c =>
                    {

                    },
                    OnResponseSignIn = c =>
                    {

                    },
                    OnResponseSignOut = c =>
                    {

                    },
                    OnValidateIdentity = c =>
                    {
                        return Task.CompletedTask;
                    }
                },
                //CookieManager = new CookieManagerCustom()
            };
            app.UseCookieAuthentication(cookieOption);
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            //OAuth2 bearer token middleware
            var resolver = ApplicationConfiguration.Instance.DependencyResolver;
            var optionProvider = resolver.Resolve<IAuthorizationServerOptionsProvider<OAuthAuthorizationServerOptions>>();
            OAuthOptions = optionProvider.GetOptions();

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            //SSOAuthenticationExtensions.UseSaml2SSOAuthentication(app, assertionEndpoints: "/api/Account/SSOLogon")
            SSOAuthenticationExtensions.UseSaml2SSOAuthentication(app);
            SSOAuthenticationExtensions.UseMetadataMiddleware(app, "/sp/metadata", MetadataType.SP, resolver);
            SSOAuthenticationExtensions.RegisterDiscoveryService(app, resolver);
            SSOAuthenticationExtensions.RegisterLogger(app, resolver);

           SLOAuthenticationExtensions.UseSaml2SLOAuthentication(app);
           //SLOAuthenticationExtensions.RegisterLogger(app,resolver);
        }
    }

    internal class CookieManagerCustom : ICookieManager
    {
        public void AppendResponseCookie(IOwinContext context, string key, string value, CookieOptions options)
        {
            //throw new System.NotImplementedException();
        }

        public void DeleteCookie(IOwinContext context, string key, CookieOptions options)
        {
            throw new System.NotImplementedException();
        }

        public string GetRequestCookie(IOwinContext context, string key)
        {
            return "Federaion";
            //throw new System.NotImplementedException();
        }
    }
}