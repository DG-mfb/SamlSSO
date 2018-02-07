using Kernel.Authorisation;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Threading.Tasks;

namespace OAuthAuthorisationService.ContextValidators
{
    internal class ResourceOwnerClientContextValidator : IContextValidator<OAuthValidateClientAuthenticationContext>
    {
        public Task ValidateContext(OAuthValidateClientAuthenticationContext context)
        {
            var grandType = context.Parameters.Get("grant_type");
            if (!String.IsNullOrWhiteSpace(grandType) && grandType.Equals("password", StringComparison.OrdinalIgnoreCase) && context.ClientId == null)
                context.Validated();
            return Task.CompletedTask;
        }
    }
}