using Kernel.DependancyResolver;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using SSOOwinMiddleware.Handlers;

namespace SSOOwinMiddleware
{
    internal class SSOOwinMiddleware : AuthenticationMiddleware<SSOAuthenticationOptions>
    {
        private readonly IDependencyResolver _resolver;
        public SSOOwinMiddleware(OwinMiddleware next, SSOAuthenticationOptions options, IDependencyResolver resolver)
            : base(next, options)
        {
            this._resolver = resolver;
        }
        
        protected override AuthenticationHandler<SSOAuthenticationOptions> CreateHandler()
        {
            return this._resolver.Resolve<SSOAuthenticationHandler>();
        }
    }
}