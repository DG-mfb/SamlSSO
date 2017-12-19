using System.Threading.Tasks;
using Kernel.Logging;

namespace Federation.Protocols.Response.Validation.ValidationRules
{
    internal class ResponseSuccessRule : ResponseValidationRule
    {
        public ResponseSuccessRule(ILogProvider logProvider) : base(logProvider)
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
            base._logProvider.LogMessage("ResponseSuccessRule running.");
            ResponseHelper.EnsureSuccessAndThrow(context.Response);
            base._logProvider.LogMessage("ResponseSuccessRule success.");
            return Task.FromResult(true);
        }
    }
}