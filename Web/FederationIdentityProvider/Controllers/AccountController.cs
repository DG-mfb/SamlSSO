using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FederationIdentityProvider.Controllers
{
    [RoutePrefix("account")]
    public class AccountController : ApiController
    {
        [Route("sso/login")]
        [HttpPost]
        public IHttpActionResult Login(string username, string password)
        {
            return Ok();
        }
    }
}