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

        public async Task<IDictionary<string, ClaimsIdentity>> GenerateUserIdentitiesAsync(Tuple<Saml2SecurityToken, HandleTokenContext> user, IEnumerable<string> authenticationTypes)
        {
            if (user == null)
                throw new ArgumentNullException("token/context");

            if (authenticationTypes == null)
                throw new ArgumentNullException("authenticationTypes");

            if (user.Item1 == null)
                throw new ArgumentNullException("saml2SecurityToken");

            if (user.Item2 == null)
                throw new ArgumentNullException("handleTokenContext");

            var configuration = this._tokenHandlerConfigurationProvider.GetTrustedIssuersConfiguration();
            base.Configuration = configuration;
            var claims = base.CreateClaims(user.Item1);
            this._logProvider.LogMessage(String.Format("Identity claims built."));

            var sessionData = user.Item1.Assertion.Statements.OfType<Saml2AuthenticationStatement>()
                .Select(x => new { x.SessionIndex, x.SessionNotOnOrAfter, Issuer = user.Item1.Assertion.Issuer.Value})
                .Where(x => !String.IsNullOrWhiteSpace(x.SessionIndex));
            if (sessionData != null && sessionData.Count() > 0)
            {
                var issuer = this._tokenHandlerConfigurationProvider.GetTrustedIssuersConfiguration().IssuerNameRegistry.GetIssuerName(user.Item1.IssuerToken);
                claims.AddClaim(new Claim(ClaimTypes.UserData, sessionData.First().SessionIndex, "string", issuer));
            }

            IDictionary<string, ClaimsIdentity> identity = authenticationTypes.ToDictionary(k => k, v => claims);
            if (this.CustomClaimsProvider != null)
            {
                this._logProvider.LogMessage(String.Format("Customising Identity claims."));
                identity = await this.CustomClaimsProvider.GenerateUserIdentitiesAsync(new ClaimsIdentityContext(claims, user.Item2.RelayState), authenticationTypes);
            }
            
            return identity;
        }
    }
}