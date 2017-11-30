using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Kernel.Authentication.Claims;

namespace WebApi.Claims
{
    public class CustomUserClaimsProvider : IUserClaimsProvider<Tuple<ClaimsIdentity, object>>
    {
        public Task<IDictionary<string, ClaimsIdentity>> GenerateUserIdentitiesAsync(Tuple<ClaimsIdentity, object> user, IEnumerable<string> authenticationTypes)
        {
            return Task.FromResult<IDictionary<string, ClaimsIdentity>>(new Dictionary<string, ClaimsIdentity>());
        }
    }
}