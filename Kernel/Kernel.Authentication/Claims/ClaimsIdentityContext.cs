using System.Security.Claims;

namespace Kernel.Authentication.Claims
{
    public class ClaimsIdentityContext
    {
        public ClaimsIdentityContext(ClaimsIdentity identity, object state)
        {
            this.ClaimsIdentity = identity;
            this.State = state;
        }
        public ClaimsIdentity ClaimsIdentity { get; }
        public object State { get; set; }
    }
}