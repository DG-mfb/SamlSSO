using System;
using System.IO;
using System.Threading.Tasks;
using JsonMetadataContextProvider.Authorization;
using JsonMetadataContextProvider.Security;
using Kernel.DependancyResolver;
using Shared.Initialisation;

namespace JsonMetadataContextProvider.Initialisation
{
    public class JsonMetadaContextProviderInitialiser : Initialiser
    {
        public override byte Order
        {
            get { return 1; }
        }

        protected override Task InitialiseInternal(IDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterType<FederationPartyContextBuilder>(Lifetime.Transient);
            dependencyResolver.RegisterType<CertificateValidationConfigurationProvider>(Lifetime.Transient);
            dependencyResolver.RegisterType<JsonAuthorizationServerConfigurationManager>(Lifetime.Transient);
            dependencyResolver.RegisterFactory<Func<string>>(() => () =>
            {
                using (var reader = new StreamReader(@"D:\Dan\Software\Temp\JsonConfiguration.txt"))
                {
                    return reader.ReadToEnd();
                }
            }, Lifetime.Singleton);
            return Task.CompletedTask;
        }
    }
}