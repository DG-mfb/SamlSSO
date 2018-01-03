using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Federation.Protocols.Request.Validation.ValidationRules;
using Kernel.Extensions;
using Kernel.Federation.Protocols.Bindings.HttpRedirectBinding;
using Kernel.Federation.Protocols.Request;
using Kernel.Logging;
using Kernel.Validation;

namespace Federation.Protocols.Request.Validation
{
    internal class RequestValidator : IRequestValidator<HttpRedirectInboundContext>
    {
        private readonly ILogProvider _logProvider;
        private readonly RuleFactory _ruleFactory;
        public RequestValidator(ILogProvider logProvider, RuleFactory ruleFactory)
        {
            this._logProvider = logProvider;
            this._ruleFactory = ruleFactory;
        }
        public Task Validate(ValidationContext context)
        {
            throw new NotImplementedException();
        }

        public async Task ValidateIRequest(HttpRedirectInboundContext context, IDictionary<string, string> form)
        {
            this._logProvider.LogMessage("Validating saml response.");
            var validationContext = new SamlRequestValidationContext(context, form);
            var rules = this._ruleFactory.GetValidationRules();
            var seed = new Func<ValidationContext, Task>(c =>
            {
                if (!c.IsValid)
                    throw new Exception(EnumerableExtensions.Aggregate(validationContext.ValidationResult.Select(x => x.ErrorMessage)));
                return Task.CompletedTask;
            });
            var del = rules
                .Reverse()
                .Aggregate(seed, (next, r) => new Func<ValidationContext, Task>(ctx => r.Validate(ctx, next)));
            await del(validationContext);
        }
    }
}
