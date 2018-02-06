using System;
using Kernel.Authorisation;
using Kernel.Initialisation;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace OAuthAuthorisationService
{
    class OAuthAuthorizationServerOptionsProvider : IAuthorizationServerOptionsProvider<OAuthAuthorizationServerOptions>
    {
        public ISecureDataFormat<AuthenticationTicket> SecureDataFormat { private get; set; }
        OAuthAuthorizationServerOptions IAuthorizationServerOptionsProvider<OAuthAuthorizationServerOptions>.GetOptions()
        {
            return new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new UserOAuthProvider(ApplicationConfiguration.Instance.DependencyResolver),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AccessTokenProvider = new OAuthTokenProvider(ApplicationConfiguration.Instance.DependencyResolver),
                AccessTokenFormat = this.SecureDataFormat != null ? this.SecureDataFormat : null,
#if(DEBUG)
                AllowInsecureHttp = true

#else
                AllowInsecureHttp = false;
#endif
            };
        }
    }
}