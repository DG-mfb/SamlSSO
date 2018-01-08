using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml;
using Federation.Protocols.Tokens;
using Kernel.Logging;
using Kernel.Security.CertificateManagement;

namespace Federation.Protocols.Response.Validation.ValidationRules
{
    internal class ResponseSignatureRule : ResponseValidationRule
    {
        private readonly ICertificateManager _certificateManager;
        public ResponseSignatureRule(ILogProvider logProvider, ICertificateManager certificateManager) : base(logProvider)
        {
            this._certificateManager = certificateManager;
        }

        internal override RuleScope Scope
        {
            get
            {
                return RuleScope.Always;
            }
        }

        protected override Task<bool> ValidateInternal(SamlResponseValidationContext context)
        {
            var inboundContext = context.ResponseContext;
            var validated = false;
            if (inboundContext.SamlInboundMessage.Binding == new Uri(Kernel.Federation.Constants.ProtocolBindings.HttpRedirect))
                validated = Helper.ValidateRedirectSignature(inboundContext, this._certificateManager);
            else
            {
                //return Task.FromResult(true);
                base._logProvider.LogMessage("ResponseSignatureRule running.");
                var cspParams = new CspParameters();
                cspParams.KeyContainerName = "XML_DSIG_RSA_KEY";
                var rsaKey = new RSACryptoServiceProvider(cspParams);
                var doc = new XmlDocument();
                doc.LoadXml(context.ResponseContext.SamlMassage);

                var signEl = TokenHelper.GetElement("Signature", "http://www.w3.org/2000/09/xmldsig#", doc.DocumentElement);
                if (signEl == null)
                    return Task.FromResult(true);

                var certEl = TokenHelper.GetElement("X509Certificate", "http://www.w3.org/2000/09/xmldsig#", signEl);
                var signedXml = new SignedXml(doc.DocumentElement);
                var dcert2 = new X509Certificate2(Convert.FromBase64String(certEl.InnerText));

                signedXml.LoadXml(signEl);
                validated = signedXml.CheckSignature(dcert2, true);

                base._logProvider.LogMessage(String.Format("ResponseSignatureRule{0}.", validated ? " success" : "failure"));
                if (!validated)
                    throw new InvalidOperationException("Invalid response signature.");
            }
            return Task.FromResult(validated);
        }
    }
}