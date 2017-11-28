using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kernel.Authentication.Claims;
using Kernel.Federation.Tokens;

namespace Federation.Protocols
{
    internal class ClaimsProvider : Saml2SecurityTokenHandler, IUserClaimsProvider<Tuple<Saml2SecurityToken, HandleTokenContext>>
    {
        private readonly ITokenConfigurationProvider<SecurityTokenHandlerConfiguration> _tokenHandlerConfigurationProvider;
        
        public ClaimsProvider(ITokenConfigurationProvider<SecurityTokenHandlerConfiguration> tokenHandlerConfigurationProvider)
        {
            this._tokenHandlerConfigurationProvider = tokenHandlerConfigurationProvider;
        }

        public Task<IDictionary<string, ClaimsIdentity>> GenerateUserIdentitiesAsync(Tuple<Saml2SecurityToken, HandleTokenContext> user, IEnumerable<string> authenticationTypes)
        {
            var configuration = this._tokenHandlerConfigurationProvider.GetTrustedIssuersConfiguration();
            base.Configuration = configuration;
            var claims = base.CreateClaims(user.Item1);
            claims.AddClaim(new Claim(ClaimTypes.Uri, user.Item2.Origin));
            var identity = new Dictionary<string, ClaimsIdentity> { { authenticationTypes.First(), claims } };
            return Task.FromResult<IDictionary<string, ClaimsIdentity>>(identity);
        }
    }
}