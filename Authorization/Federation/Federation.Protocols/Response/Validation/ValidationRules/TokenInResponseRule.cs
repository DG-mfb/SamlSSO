using System;
using System.Threading.Tasks;
using Kernel.Logging;

namespace Federation.Protocols.Response.Validation.ValidationRules
{
    internal class TokenInResponseRule : ResponseValidationRule
    {
        public TokenInResponseRule(ILogProvider logProvider)
            : base(logProvider)
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
            base._logProvider.LogMessage("TokenInResponseRule In Response Rule running.");
            var tokenResponse = context.ResponseContext.StatusResponse as Shared.Federtion.Response.TokenResponse;
            if (tokenResponse == null)
                return Task.FromResult(true);

            var hasToken = (tokenResponse != null && tokenResponse.Assertions != null && tokenResponse.Assertions.Length == 1);
            if (context.ResponseContext.IsSuccess && !hasToken)
            {
                throw new InvalidOperationException("Security token is missing.");
            }
            return Task.FromResult(true);
        }
    }
}