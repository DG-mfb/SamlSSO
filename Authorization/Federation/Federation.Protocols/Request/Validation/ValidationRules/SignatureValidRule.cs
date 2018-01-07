using System;
using System.Linq;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Kernel.Logging;
using Kernel.Security.CertificateManagement;

namespace Federation.Protocols.Request.Validation.ValidationRules
{
    internal class SignatureValidRule : RequestValidationRule
    {
        private readonly ICertificateManager _certificateManager;
        public SignatureValidRule(ILogProvider logProvider, ICertificateManager certificateManager) : base(logProvider)
        {
            this._certificateManager = certificateManager;
        }
        
        protected override Task<bool> ValidateInternal(SamlRequestValidationContext context)
        {
            throw new NotImplementedException();
            //var inboundContext = context.InboundContext;
            //var validated = false;
            //foreach (var k in inboundContext.Keys.SelectMany(x => x.KeyInfo))
            //{
            //    var binaryClause = k as BinaryKeyIdentifierClause;
            //    if (binaryClause == null)
            //        throw new InvalidOperationException(String.Format("Expected type: {0} but it was: {1}", typeof(BinaryKeyIdentifierClause), k.GetType()));

            //    var certContent = binaryClause.GetBuffer();
            //    var cert = new X509Certificate2(certContent);
            //    validated = this.VerifySignature(inboundContext.Request, cert);
            //    if (validated)
            //        break;
            //}
            //return Task.FromResult(validated);
        }

        private bool VerifySignature(string request, X509Certificate2 certificate)
        {
            var i = request.IndexOf("Signature");
            var data = request.Substring(0, i - 1);
            var sgn = Uri.UnescapeDataString(request.Substring(i + 10));
            
            var validated = this._certificateManager.VerifySignatureFromBase64(data, sgn, certificate);
            return validated;
        }
    }
}