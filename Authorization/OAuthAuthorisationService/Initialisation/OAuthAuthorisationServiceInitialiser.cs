using System.Threading.Tasks;
using Kernel.DependancyResolver;
using OAuthAuthorisationService.ContextValidators;
using Shared.Initialisation;

namespace OAuthAuthorisationService.Initialisation
{
    public class OAuthAuthorisationServiceInitialiser : Initialiser
    {
        public override byte Order
        {
            get { return 0; }
        }

        protected override Task InitialiseInternal(IDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterType<OAuthAuthorizationServerOptionsProvider>(Lifetime.Transient);
            dependencyResolver.RegisterType<UserOAuthProvider>(Lifetime.Transient);
            dependencyResolver.RegisterType<ResourceOwnerClientContextValidator>(Lifetime.Transient);
            return Task.CompletedTask;
        }
    }
}