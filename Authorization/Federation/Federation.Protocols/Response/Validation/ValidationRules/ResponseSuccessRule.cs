using System;
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
            try
            {
                ResponseHelper.EnsureSuccessAndThrow(context.Response);
                return Task.FromResult(true);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
