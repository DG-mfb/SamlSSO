using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Kernel.Authorisation;
using Microsoft.Owin.Security.OAuth;

namespace WebApi.Token
{
    public class ClientTokenGrantService : ITokenGrantService<OAuthGrantClientCredentialsContext>
    {
        public Task GrantToken(OAuthGrantClientCredentialsContext context)
        {
            var identity = new ClaimsIdentity(new GenericIdentity(context.ClientId));
            context.Validated(identity);
            return Task.CompletedTask;
        }
    }
}