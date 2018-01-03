using Kernel.Authorisation;
using Kernel.Federation.MetaData;
using Kernel.Initialisation;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SLOOwinMiddleware.Extensions;
using SSOOwinMiddleware.Extensions;

namespace WebApi
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

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
}