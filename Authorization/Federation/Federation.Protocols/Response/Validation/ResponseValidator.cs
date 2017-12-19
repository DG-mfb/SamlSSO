using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Federation.Protocols.Response.Validation.ValidationRules;
using Kernel.Extensions;
using Kernel.Federation.Protocols.Response;
using Kernel.Initialisation;
using Kernel.Logging;
using Kernel.Validation;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response.Validation
{
    internal class ResponseValidator : IResponseValidator<ResponseStatus>
    {
        private readonly ILogProvider _logProvider;

        public ResponseValidator(ILogProvider logProvider)
        {
            this._logProvider = logProvider;
        }
        public async Task ValidateResponse(ResponseStatus response, IDictionary<string, string> form)
        {
            this._logProvider.LogMessage("Validating saml response.");
            var context = new SamlResponseValidationContext(response, form);
            var factory = ApplicationConfiguration.Instance.DependencyResolver.Resolve<RuleFactory>();
            var rules = factory.GetValidationRules(r => r.Scope == (response.IsIdpInitiated ? RuleScope.IdpInitiated : RuleScope.SPInitiated));
            var seed = new Func<ValidationContext, Task>(c =>
            {
                if (!c.IsValid)
                    throw new Exception(EnumerableExtensions.Aggregate(context.ValidationResult.Select(x => x.ErrorMessage)));
                return Task.CompletedTask;
            });
            var del = rules
                .Reverse()
                .Aggregate(seed, (next, r) => new Func<ValidationContext, Task>(ctx => r.Validate(ctx, next)));
            await del(context);
        }

        Task IValidator.Validate(Kernel.Validation.ValidationContext context)
        {
            throw new NotImplementedException();
        }
    }
}