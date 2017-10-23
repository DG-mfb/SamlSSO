using System.Threading.Tasks;
using Kernel.DependancyResolver;
using Shared.Initialisation;

namespace Microsoft.Owin.CertificateValidators.Initialisation
{
    public class BackchannelValidatorsInitialiser : Initialiser
    {
        public override byte Order
        {
            get { return 0; }
        }

        protected override Task InitialiseInternal(IDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterType<CertificateValidatorResolver>(Lifetime.Transient);
            return Task.CompletedTask;
        }
    }
}