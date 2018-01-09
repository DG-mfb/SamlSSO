using System;
using System.Threading.Tasks;
using Kernel.Cryptography.Signing.Xml;
using Kernel.Logging;
using Kernel.Security.CertificateManagement;

namespace Federation.Protocols.Request.Validation.ValidationRules
{
    internal class SignatureValidRule : RequestValidationRule
    {
        private readonly ICertificateManager _certificateManager;
        private readonly IXmlSignatureManager _signatureManager;
        public SignatureValidRule(ILogProvider logProvider, ICertificateManager certificateManager, IXmlSignatureManager signatureManager) : base(logProvider)
        {
            this._certificateManager = certificateManager;
            this._signatureManager = signatureManager;
        }
        
        protected override Task<bool> ValidateInternal(SamlRequestValidationContext context)
        {
            var inboundContext = context.RequestContext;
            var validated = false;
            if (inboundContext.SamlInboundMessage.Binding == new Uri(Kernel.Federation.Constants.ProtocolBindings.HttpRedirect))
                validated = Helper.ValidateRedirectSignature(inboundContext, this._certificateManager);
            else
                validated = Helper.ValidateMessageSignature(inboundContext, this._signatureManager);
            
            if (!validated)
                throw new InvalidOperationException("Invalid signature.");
            context.RequestContext.Validated();
            return Task.FromResult(validated);
        }
    }
}