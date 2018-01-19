using Kernel.Initialisation;
using Microsoft.Owin.Security;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApi.Results
{
    public class CookieRedirectResult : IHttpActionResult
    {
        private readonly Uri _location;
        private readonly HttpRequestMessage _request;
        private ClaimsIdentity _identity;

        public CookieRedirectResult(Uri location, HttpRequestMessage request, ClaimsIdentity identity)
        {
            if (location == (Uri)null)
                throw new ArgumentNullException(nameof(location));
            this._location = location;
            this._request = request;
            this._identity = identity;
        }

        public Uri Location
        {
            get
            {
                return this._location;
            }
        }

        public HttpRequestMessage Request
        {
            get
            {
                return this._request;
            }
        }

        public virtual Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<HttpResponseMessage>(this.Execute());
        }

        private HttpResponseMessage Execute()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Found);
            try
            {
                var resolver = ApplicationConfiguration.Instance.DependencyResolver;
                var protector = resolver.Resolve<ISecureDataFormat<AuthenticationTicket>>();

                var ticket = new AuthenticationTicket(this._identity, new AuthenticationProperties());
                var token = protector.Protect(ticket);
                var cookie = new CookieHeaderValue("Bearer", token);
                cookie.Expires = DateTimeOffset.Now.AddDays(1);
                cookie.Domain = Request.RequestUri.Host;
                cookie.Path = "/";
                httpResponseMessage.Headers.AddCookies(new[] { cookie });
                httpResponseMessage.Headers.Location = this._location;
                httpResponseMessage.RequestMessage = this._request;
            }
            catch
            {
                httpResponseMessage.Dispose();
                throw;
            }
            return httpResponseMessage;
        }
    }
}