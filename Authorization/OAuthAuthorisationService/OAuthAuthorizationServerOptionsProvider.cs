using System;
using System.Threading.Tasks;
using Kernel.Authorisation;
using Kernel.Initialisation;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;

namespace OAuthAuthorisationService
{
    class OAuthAuthorizationServerOptionsProvider : IAuthorizationServerOptionsProvider<OAuthAuthorizationServerOptions>
    {
        OAuthAuthorizationServerOptions IAuthorizationServerOptionsProvider<OAuthAuthorizationServerOptions>.GetOptions()
        {
            return new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new UserOAuthProvider(ApplicationConfiguration.Instance.DependencyResolver),//new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AccessTokenProvider = new AuthenticationTokenProvider { OnReceiveAsync = c =>
                {
                    return Task.CompletedTask;
                }},
                
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true
            };
        }
    }
}