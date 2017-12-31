using System.IdentityModel.Tokens;
using System.Security.Claims;

namespace Kernel.Authentication.Claims
{
    public class ClaimsIdentityContext
    {
        public ClaimsIdentityContext(ClaimsIdentity identity, Saml2Assertion saml2Assertion, object state)
        {
            this.ClaimsIdentity = identity;
            this.Saml2Assertion = saml2Assertion;
            this.State = state;
        }
        public Saml2Assertion Saml2Assertion { get; }
        public ClaimsIdentity ClaimsIdentity { get; }
        public object State { get; set; }
    }
}