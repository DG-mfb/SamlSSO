using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FederationIdentityProvider.Controllers
{
    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        [Route("authenticate")]
        [HttpPost]
        public IHttpActionResult Login([FromBody]LoginModel loginModel)
        {
            return Ok();
        }
    }
}