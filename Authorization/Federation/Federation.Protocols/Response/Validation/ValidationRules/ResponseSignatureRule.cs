using System;
using System.Threading.Tasks;
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
                validated = Helper.ValidateMessageSignature(inboundContext, this._certificateManager);
            }

            base._logProvider.LogMessage(String.Format("ResponseSignatureRule{0}.", validated ? " success" : "failure"));
            if (!validated)
                throw new InvalidOperationException("Invalid response signature.");

            return Task.FromResult(validated);
        }
    }
}