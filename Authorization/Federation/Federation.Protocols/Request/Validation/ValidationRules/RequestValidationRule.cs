using System;
using System.Threading.Tasks;
using Kernel.Logging;
using Kernel.Validation;

namespace Federation.Protocols.Request.Validation.ValidationRules
{
    internal abstract class RequestValidationRule : IValidationRule
    {
        protected readonly ILogProvider _logProvider;
        public RequestValidationRule(ILogProvider logProvider)
        {
            this._logProvider = logProvider;
        }

        public async Task Validate(ValidationContext context, Func<ValidationContext, Task> next)
        {
            try
            {
                if (await this.ValidateInternal((SamlRequestValidationContext)context))
                    await next(context);
            }
            catch(Exception e)
            {
                Exception inner;
                this._logProvider.TryLogException(e, out inner);
                throw;
            }
        }

        protected abstract Task<bool> ValidateInternal(SamlRequestValidationContext context);
    }
}