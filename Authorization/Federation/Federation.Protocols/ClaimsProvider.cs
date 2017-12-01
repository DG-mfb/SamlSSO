using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kernel.Authentication.Claims;
using Kernel.Federation.Tokens;
using Kernel.Logging;

namespace Federation.Protocols
{
    internal class ClaimsProvider : Saml2SecurityTokenHandler, IUserClaimsProvider<Tuple<Saml2SecurityToken, HandleTokenContext>>
    {
        private readonly ITokenConfigurationProvider<SecurityTokenHandlerConfiguration> _tokenHandlerConfigurationProvider;
        private readonly ILogProvider _logProvider;
        public IUserClaimsProvider<ClaimsIdentityContext> CustomClaimsProvider { private get;  set; }
        public ClaimsProvider(ITokenConfigurationProvider<SecurityTokenHandlerConfiguration> tokenHandlerConfigurationProvider,  ILogProvider logProvider)
        {
            this._logProvider = logProvider;
            this._tokenHandlerConfigurationProvider = tokenHandlerConfigurationProvider;
        }

        public Task<IDictionary<string, ClaimsIdentity>> GenerateUserIdentitiesAsync(Tuple<Saml2SecurityToken, HandleTokenContext> user, IEnumerable<string> authenticationTypes)
        {
            var configuration = this._tokenHandlerConfigurationProvider.GetTrustedIssuersConfiguration();
            base.Configuration = configuration;
            var claims = base.CreateClaims(user.Item1);
            this._logProvider.LogMessage(String.Format("Identity claims built."));
            claims.AddClaim(new Claim(ClaimTypes.Uri, user.Item2.Origin));
            if (this.CustomClaimsProvider != null)
            {
                this._logProvider.LogMessage(String.Format("Customising Identity claims."));
                this.CustomClaimsProvider.GenerateUserIdentitiesAsync(new ClaimsIdentityContext(claims, user.Item2.RelayState), authenticationTypes);
            }
            var identity = new Dictionary<string, ClaimsIdentity> { { authenticationTypes.First(), claims } };
            return Task.FromResult<IDictionary<string, ClaimsIdentity>>(identity);
        }
    }
}