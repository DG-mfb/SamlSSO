using System.Threading.Tasks;
using Kernel.Cryptography.Validation;
using Kernel.DependancyResolver;
using SecurityManagement.CertificateValidationRules;
using Shared.Initialisation;

namespace SecurityManagement.Initialisation
{
    public class SecurityInitialiser : Initialiser
    {
        public override byte Order
        {
            get { return 1; }
        }

        protected override Task InitialiseInternal(IDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterType<CertificateManager>(Lifetime.Transient);
            dependencyResolver.RegisterType<XmlSignatureManager>(Lifetime.Transient);
            dependencyResolver.RegisterType<CertificateValidator>(Lifetime.Transient);
            dependencyResolver.RegisterType<BackchannelCertificateValidator>(Lifetime.Transient);
            CertificateValidationRulesFactory.InstanceCreator = t => (ICertificateValidationRule)dependencyResolver.Resolve(t);
            return Task.CompletedTask;
        }
    }
}