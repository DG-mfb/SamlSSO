using Microsoft.Owin.Security;

namespace SLOOwinMiddleware
{
    public class SLOAuthenticationOptions : AuthenticationOptions
    {
        public string Caption
        {
            get
            {
                return base.Description.Caption;
            }
            set
            {
                base.Description.Caption = value;
            }
        }
        
        public SLOAuthenticationOptions()
            : base("Saml2SLO")
        {
            this.Caption = "Saml2SLO";
            base.AuthenticationMode = AuthenticationMode.Active;
        }
    }
}