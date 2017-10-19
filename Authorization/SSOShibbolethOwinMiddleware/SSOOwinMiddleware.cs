using Kernel.DependancyResolver;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using SSOOwinMiddleware.Handlers;

namespace SSOOwinMiddleware
{
    internal class SSOOwinMiddleware : AuthenticationMiddleware<SSOAuthenticationOptions>
    {
        private readonly ILogger _logger;
        private readonly IDependencyResolver _resolver;
        public SSOOwinMiddleware(OwinMiddleware next, IAppBuilder app, SSOAuthenticationOptions options, IDependencyResolver resolver)
            : base(next, options)
        {
            this._resolver = resolver;
            this._logger = app.CreateLogger<SSOOwinMiddleware>();
        }
        
        protected override AuthenticationHandler<SSOAuthenticationOptions> CreateHandler()
        {
            return new SSOAuthenticationHandler(this._logger, this._resolver);
        }
    }
}