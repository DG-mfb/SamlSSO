using Kernel.DependancyResolver;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using SLOOwinMiddleware.Handlers;

namespace SLOOwinMiddleware
{
    internal class SLOOwinMiddleware : AuthenticationMiddleware<SLOAuthenticationOptions>
    {
        private readonly IDependencyResolver _resolver;
        public SLOOwinMiddleware(OwinMiddleware next, SLOAuthenticationOptions options, IDependencyResolver resolver)
            : base(next, options)
        {
            this._resolver = resolver;
        }
        
        protected override AuthenticationHandler<SLOAuthenticationOptions> CreateHandler()
        {
            return this._resolver.Resolve<SLOAuthenticationHandler>();
        }
    }
}