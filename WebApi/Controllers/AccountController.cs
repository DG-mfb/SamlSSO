using Kernel.Initialisation;
using Microsoft.Owin.Security;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.Results;

namespace WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        [Route("SSOLogon")]
        public async Task<IHttpActionResult> SSOLogon()
        {
            var resolver = ApplicationConfiguration.Instance.DependencyResolver;
            var protector = resolver.Resolve<ISecureDataFormat<AuthenticationTicket>>();
            
            var identity = (ClaimsIdentity)User.Identity;
            var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            var token = protector.Protect(ticket);
            var redirectUrl = String.Format("http://localhost:49974/Logout.aspx?Bearer={0}", token);
            //return Redirect(String.Format("http://localhost:49974/Logout.aspx?Bearer={0}", token));
            return new CookieRedirectResult(new Uri(redirectUrl), new HttpRequestMessage(Request.Method, Request.RequestUri.AbsoluteUri), (ClaimsIdentity)User.Identity);
            
            //FormsAuthentication.SetAuthCookie("testUser", false);
            //HttpContext.Current.Response.Headers.Add("Authorization", "Bearer " + token);
            //HttpContext.Current.Response.Cookies.Add(new HttpCookie("Bearer", token));
            //return Redirect(new Uri("http://localhost:49974/Logout.aspx"));
            //throw new NotImplementedException();
            //return Ok();
            //var context = System.Web.HttpContextExtensions.GetOwinContext(HttpContext.Current.Request);
            //var samlAuthType = await context.Authentication.AuthenticateAsync("Saml2SSO");

            //var protector = new DpapiDataProtectionProvider("SSO server").Create("token");
            //var dataFormat = new TicketDataFormat(protector);

            //var identity = (ClaimsIdentity)base.RequestContext.Principal.Identity;
            //var ticket = new AuthenticationTicket(identity, samlAuthType.Properties);
            //var serialisedTicket = dataFormat.Protect(ticket);
            //var targetUri = string.Format("http://localhost:61463/api/SSO?token={0}", serialisedTicket);
            //return Redirect(targetUri);
            //return Ok(serialisedTicket);
            //var origin = identity.FindFirst(ClaimTypes.Uri).Value;
            //var identityClaim = identity.Claims.Where(c => c.Value.Contains("@"))
            //    .FirstOrDefault(c => c.Subject.NameClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            //if(identityClaim != null)
            //    return Ok(identityClaim);
            //return Ok(identity.Claims);
        }

        [Route("SSOLogout")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> SSOLogout()
        {
            return Ok("Logged out.");
        }
    }
}