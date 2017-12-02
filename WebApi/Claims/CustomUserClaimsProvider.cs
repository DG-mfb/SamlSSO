using System;
using System.Collections.Generic;
using System.Linq;
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
            if (user == null)
                throw new ArgumentNullException("context");

            if (authenticationTypes == null)
                throw new ArgumentNullException("authenticationTypes");

            return Task.FromResult<IDictionary<string, ClaimsIdentity>>(authenticationTypes.ToDictionary(k => k, v => user.ClaimsIdentity));
        }
    }
}