using System.Threading.Tasks;
using Kernel.DependancyResolver;
using Microsoft.Owin.Security.Infrastructure;

namespace OAuthAuthorisationService
{
    internal class OAuthTokenProvider : AuthenticationTokenProvider
    {
        private readonly IDependencyResolver dependencyResolver;
        public OAuthTokenProvider(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public override Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}