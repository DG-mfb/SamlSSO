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
            if (response.IsIdpInitiated)
                return;
            var context = new SamlResponseValidationContext(response, form);
            var factory = ApplicationConfiguration.Instance.DependencyResolver.Resolve<RuleFactory>();
            var rules = factory.GetValidationRules(r => r.Scope == (response.IsIdpInitiated ? RuleScope.IdpInitiated : RuleScope.SPInitiated));
            foreach(var r in rules)
            {
                await r.Validate(context, next => Task.CompletedTask);
            }
           
            if (!context.IsValid)
                throw new Exception(EnumerableExtensions.Aggregate(context.ValidationResult.Select(x => x.ErrorMessage)));
        }

        Task IValidator.Validate(Kernel.Validation.ValidationContext context)
        {
            throw new NotImplementedException();
        }
    }
}