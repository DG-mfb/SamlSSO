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
            var emails = identity.Claims.Where(c => c.Value.Contains("@"))
                .First(c => c.Subject.NameClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            return Ok(emails);
        }
    }
}