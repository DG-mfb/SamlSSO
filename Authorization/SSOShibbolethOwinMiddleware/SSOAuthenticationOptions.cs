using System.Collections.Generic;
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
                return base.Description.Caption;
            }
            set
            {
                base.Description.Caption = value;
            }
        }
        
       

        public PathString SSOPath { get; set; }

        public ICollection<PathString> AssertionEndpoinds { get; }
        public SSOAuthenticationOptions()
            : base("Saml2SSO")
        {
            this.Caption = "Saml2SSO";
            base.AuthenticationMode = AuthenticationMode.Active;
            this.SSOPath = new PathString("/account/sso");
            this.AssertionEndpoinds = new List<PathString>();
        }
    }
}