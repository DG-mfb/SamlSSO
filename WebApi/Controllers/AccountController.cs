using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using System.Web;
using Microsoft.Owin.Security;

namespace WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        [Route("SSOLogon")]
        public async Task<IHttpActionResult> SSOLogon()
        {
            var context = System.Web.HttpContextExtensions.GetOwinContext(HttpContext.Current.Request);
            var samlAuthType = await context.Authentication.AuthenticateAsync("Saml2SSO");

            var protector = new DpapiDataProtectionProvider("SSO server").Create("token");
            var dataFormat = new TicketDataFormat(protector);
           
            var identity = (ClaimsIdentity)base.RequestContext.Principal.Identity;
            var ticket = new AuthenticationTicket(identity, samlAuthType.Properties);
            var serialisedTicket = dataFormat.Protect(ticket);
            var targetUri = string.Format("http://localhost:61463/api/SSO?token={0}", serialisedTicket);
            return Redirect(targetUri);
            //return Ok(serialisedTicket);
            //var origin = identity.FindFirst(ClaimTypes.Uri).Value;
            //var identityClaim = identity.Claims.Where(c => c.Value.Contains("@"))
            //    .FirstOrDefault(c => c.Subject.NameClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            //if(identityClaim != null)
            //    return Ok(identityClaim);
            //return Ok(identity.Claims);
        }
    }
}