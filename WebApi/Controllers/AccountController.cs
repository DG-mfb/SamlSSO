using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;

namespace WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        [Route("SSOLogon")]
        public async Task<IHttpActionResult> SSOLogon()
        {
            var identity = (ClaimsIdentity)base.RequestContext.Principal.Identity;
            var origin = identity.FindFirst(ClaimTypes.Uri).Value;
            var identityClaim = identity.Claims.Where(c => c.Value.Contains("@"))
                .FirstOrDefault(c => c.Subject.NameClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            if(identityClaim != null)
                return Ok(identityClaim);
            return Ok(identity.Claims);
        }
    }
}