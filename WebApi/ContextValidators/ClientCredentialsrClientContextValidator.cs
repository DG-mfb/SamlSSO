using System;
using System.Threading.Tasks;
using Kernel.Authorisation;
using Microsoft.Owin.Security.OAuth;

namespace WebApi.ContextValidators
{
    internal class ClientCredentialsrClientContextValidator : IContextValidator<OAuthValidateClientAuthenticationContext>
    {
        public Task ValidateContext(OAuthValidateClientAuthenticationContext context)
        {
            var grandType = context.Parameters.Get("grant_type");
            if (!String.IsNullOrWhiteSpace(grandType) && grandType.Equals("client_credentials", StringComparison.OrdinalIgnoreCase))
            {
                string clientId;
                string clientSecret;
                if (context.TryGetBasicCredentials(out clientId, out clientSecret))
                {
                    context.Validated(clientId);
                }
            }
            return Task.CompletedTask;
        }
    }
}