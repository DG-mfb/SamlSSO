using System;
using System.Linq;
using System.Threading.Tasks;
using Federation.Protocols.Request.Validation.ValidationRules;
using Kernel.Extensions;
using Kernel.Federation.Protocols.Request;
using Kernel.Logging;
using Kernel.Validation;
using Shared.Federtion.Request;

namespace Federation.Protocols.Request.Validation
{
    internal class RequestValidator : IRequestValidator<SamlInboundRequestContext>
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

        public async Task ValidateIRequest(SamlInboundRequestContext request)
        {
            this._logProvider.LogMessage("Validating saml response.");
            var validationContext = new SamlRequestValidationContext(request);
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