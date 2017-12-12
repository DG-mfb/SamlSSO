using System;
using System.Threading.Tasks;
using Kernel.Logging;
using Kernel.Validation;

namespace Federation.Protocols.Response.Validation.ValidationRules
{
    internal abstract class ResponseValidationRule : IValidationRule
    {
        protected readonly ILogProvider _logProvider;
        internal abstract RuleScope Scope { get; }
        public ResponseValidationRule(ILogProvider logProvider)
        {
            this._logProvider = logProvider;
        }

        public async Task Validate(ValidationContext context, Func<ValidationContext, Task> next)
        {
            try
            {
                if (await this.ValidateInternal((SamlResponseValidationContext)context))
                    await next(context);
            }
            catch(Exception e)
            {
                Exception inner;
                this._logProvider.TryLogException(e, out inner);
                throw;
            }
        }

        protected abstract Task<bool> ValidateInternal(SamlResponseValidationContext context);
    }
}