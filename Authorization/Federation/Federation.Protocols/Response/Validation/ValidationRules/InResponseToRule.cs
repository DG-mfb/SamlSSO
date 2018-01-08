using System;
using System.Threading.Tasks;
using Kernel.Logging;

namespace Federation.Protocols.Response.Validation.ValidationRules
{
    internal class InResponseToRule : ResponseValidationRule
    {
        public InResponseToRule(ILogProvider logProvider)
            : base(logProvider)
        {
            
        }

        internal override RuleScope Scope
        {
            get
            {
                return RuleScope.SPInitiated;
            }
        }

        protected override Task<bool> ValidateInternal(SamlResponseValidationContext context)
        {
            base._logProvider.LogMessage("In Response To Rule running.");
            if (context.ResponseContext.SamlInboundMessage.RelayState == null)
            {
                throw new InvalidOperationException("Relay state is missing.");
            }
            ResponseHelper.EnsureRequestIdMatch(context.ResponseContext.SamlInboundMessage.RelayState, context.ResponseContext.StatusResponse.InResponseTo);
            return Task.FromResult(true);
        }
    }
}