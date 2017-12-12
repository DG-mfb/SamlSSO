using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kernel.Extensions;
using Kernel.Federation.Protocols.Response;
using Kernel.Validation;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response.Validation
{
    internal class IdpInitResponseValidator : IResponseValidator<ResponseStatus>
    {
        public async Task ValidateResponse(ResponseStatus response, IDictionary<string, string> form)
        {
            if (!response.IsIdpInitiated)
                return;

            var context = new SamlResponseValidationContext(response, form);
            await this.Validate(context);
            if (!context.IsValid)
                throw new Exception(EnumerableExtensions.Aggregate(context.ValidationResult.Select(x => x.ErrorMessage)));
        }

        public Task Validate(ValidationContext context)
        {
            throw new NotImplementedException();
        }
    }
}