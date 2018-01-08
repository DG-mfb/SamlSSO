using System;
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
            var inboundContext = context.RequestContext;
            var validated = false;
            if (inboundContext.SamlInboundMessage.Binding == new Uri(Kernel.Federation.Constants.ProtocolBindings.HttpRedirect))
                validated = Helper.ValidateRedirectSignature(inboundContext, this._certificateManager);
            if (!validated)
                throw new InvalidOperationException("Invalid signature.");
            return Task.FromResult(validated);
        }
    }
}