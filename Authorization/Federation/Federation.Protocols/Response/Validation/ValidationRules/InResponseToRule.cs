using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Kernel.Federation.Protocols;
using Kernel.Logging;

namespace Federation.Protocols.Response.Validation.ValidationRules
{
    internal class InResponseToRule : ResponseValidationRule
    {
        private readonly IRelayStateHandler _relayStateHandler;

        public InResponseToRule(IRelayStateHandler relayStateHandler, ILogProvider logProvider)
            : base(logProvider)
        {
            this._relayStateHandler = relayStateHandler;
        }

        internal override RuleScope Scope
        {
            get
            {
                return RuleScope.SPInitiated;
            }
        }

        protected override async Task<bool> ValidateInternal(SamlResponseValidationContext context)
        {
            if (context.Response.RelayState == null)
            {
                var relayState = await this._relayStateHandler.GetRelayStateFromFormData(context.Form);
                if (relayState == null)
                {
                    context.ValidationResult.Add(new ValidationResult("Relay state is missing in the response."));
                    return false;
                }
                context.Response.RelayState = relayState;
            }
            ResponseHelper.EnsureRequestIdMatch(context.Response.RelayState, context.Response.InResponseTo);
            return true;
        }
    }
}