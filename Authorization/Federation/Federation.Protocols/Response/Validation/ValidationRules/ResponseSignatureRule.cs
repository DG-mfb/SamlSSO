using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml;
using Federation.Protocols.Tokens;
using Kernel.Logging;

namespace Federation.Protocols.Response.Validation.ValidationRules
{
    internal class ResponseSignatureRule : ResponseValidationRule
    {
        public ResponseSignatureRule(ILogProvider logProvider) : base(logProvider)
        {
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
            return Task.FromResult(true);
            base._logProvider.LogMessage("ResponseSignatureRule running.");
            var cspParams = new CspParameters();
            cspParams.KeyContainerName = "XML_DSIG_RSA_KEY";
            var rsaKey = new RSACryptoServiceProvider(cspParams);
            var doc = new XmlDocument();
            doc.LoadXml(context.Response.Response);
            
            var signEl = TokenHelper.GetElement("Signature", "http://www.w3.org/2000/09/xmldsig#", doc.DocumentElement);
            if (signEl == null)
                return Task.FromResult(true);

            var certEl = TokenHelper.GetElement("X509Certificate", "http://www.w3.org/2000/09/xmldsig#", signEl);
            var signedXml = new SignedXml(doc.DocumentElement);
            var dcert2 = new X509Certificate2(Convert.FromBase64String(certEl.InnerText));

            signedXml.LoadXml(signEl);
            var valid = signedXml.CheckSignature(dcert2, true);
            
            base._logProvider.LogMessage(String.Format("ResponseSignatureRule{0}.",valid ? " success": "failure"));
            if (!valid)
                throw new InvalidOperationException("Invalid response signature.");
            return Task.FromResult(valid);
        }
    }
}