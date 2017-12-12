using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Kernel.Federation.Protocols;
using Kernel.Logging;

namespace Federation.Protocols.Response.Validation.ValidationRules
{
    internal class IssuerKnownRule : ResponseValidationRule
    {
        private readonly IRelayStateHandler _relayStateHandler;

        public IssuerKnownRule(IRelayStateHandler relayStateHandler, ILogProvider logProvider)
            : base(logProvider)
        {
            this._relayStateHandler = relayStateHandler;
        }

        internal override RuleScope Scope
        {
            get
            {
                return RuleScope.IdpInitiated;
            }
        }

        protected override async Task<bool> ValidateInternal(SamlResponseValidationContext context)
        {
            try
            {
                
                return true;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}