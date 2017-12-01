using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Kernel.Authentication.Claims;
using Kernel.Logging;

namespace WebApi.Claims
{
    public class CustomUserClaimsProvider : IUserClaimsProvider<ClaimsIdentityContext>
    {
        private readonly ILogProvider _logProvider;
        public CustomUserClaimsProvider(ILogProvider logProvider)
        {
            this._logProvider = logProvider;
        }

        public Task<IDictionary<string, ClaimsIdentity>> GenerateUserIdentitiesAsync(ClaimsIdentityContext user, IEnumerable<string> authenticationTypes)
        {
            return Task.FromResult<IDictionary<string, ClaimsIdentity>>(new Dictionary<string, ClaimsIdentity>());
        }
    }
}