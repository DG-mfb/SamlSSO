using System;
using System.Net.Http;
using Kernel.Security.Validation;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace SSOOwinMiddleware
{
    public class SSOAuthenticationOptions : AuthenticationOptions
    {
        public IBackchannelCertificateValidator BackchannelCertificateValidator { get; set; }
        

        public HttpMessageHandler BackchannelHttpHandler { get; set; }

        public TimeSpan BackchannelTimeout { get; set; }

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
        
        public PathString CallbackPath { get; set; }

        public PathString SPMetadataPath { get; set; }

        public PathString SSOPath { get; set; }

        public SSOAuthenticationOptions()
      : base("Saml2SSO")
    {
            this.Caption = "Saml2SSO";
            this.AuthenticationMode = AuthenticationMode.Active;
            this.BackchannelTimeout = TimeSpan.FromMinutes(1.0);
            this.SPMetadataPath = new PathString("/sp/metadata");
            this.SSOPath = new PathString("/account/sso");
        }
    }
}