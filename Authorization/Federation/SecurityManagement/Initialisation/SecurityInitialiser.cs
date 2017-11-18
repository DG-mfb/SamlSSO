using System.Threading.Tasks;
using Kernel.DependancyResolver;
using Kernel.Security.Validation;
using SecurityManagement.BackchannelCertificateValidationRules;
using SecurityManagement.CertificateValidationRules;
using SecurityManagement.Signing;
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
            dependencyResolver.RegisterType<XmlSignatureManager>(Lifetime.Transient);
            dependencyResolver.RegisterType<CertificateManager>(Lifetime.Transient);
            dependencyResolver.RegisterType<CertificateValidator>(Lifetime.Transient);
            dependencyResolver.RegisterType<BackchannelCertificateValidator>(Lifetime.Transient);
            CertificateValidationRulesFactory.InstanceCreator = t => (ICertificateValidationRule)dependencyResolver.Resolve(t);
            BackchannelCertificateValidationRulesFactory.InstanceCreator = t => (IBackchannelCertificateValidationRule)dependencyResolver.Resolve(t);
            BackchannelCertificateValidationRulesFactory.CertificateValidatorResolverFactory = t => (ICertificateValidatorResolver)dependencyResolver.Resolve(t);
            return Task.CompletedTask;
        }
    }
}