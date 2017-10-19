using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace SSOOwinMiddleware
{
    public class SSOAuthenticationOptions : AuthenticationOptions
    {
        public string Caption
        {
            get
            {
                return this.Description.Caption;
            }
            set
            {
                this.Description.Caption = value;
            }
        }
        
        public PathString SPMetadataPath { get; set; }

        public PathString SSOPath { get; set; }

        public SSOAuthenticationOptions()
      : base("Saml2SSO")
    {
            this.Caption = "Saml2SSO";
            this.AuthenticationMode = AuthenticationMode.Active;
            this.SPMetadataPath = new PathString("/sp/metadata");
            this.SSOPath = new PathString("/account/sso");
        }
    }
}