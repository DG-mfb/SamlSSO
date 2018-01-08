using System;
using System.Threading.Tasks;
using Kernel.Logging;

namespace Federation.Protocols.Response.Validation.ValidationRules
{
    internal class RelayStateInResponseRule : ResponseValidationRule
    {
        public RelayStateInResponseRule(ILogProvider logProvider)
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
            base._logProvider.LogMessage("RelayState In Response Rule running.");
            if (!context.ResponseContext.SamlInboundMessage.HasRelaySate)
            {
                throw new InvalidOperationException("Relay state is missing.");
            }
            return Task.FromResult(true);
        }
    }
}