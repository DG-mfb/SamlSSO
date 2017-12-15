using System;
using System.IO;
using System.Threading.Tasks;
using JsonMetadataContextProvider.Authorization;
using JsonMetadataContextProvider.Security;
using Kernel.Configuration;
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
            dependencyResolver.RegisterFactory<Func<Type,string>>(() => t =>
            {
                string path = null;
                if(t == typeof(FederationPartyContextBuilder))
                    path = AppSettingsConfigurationManager.GetSetting("federationConfigurationFile", String.Empty);
                if (t == typeof(CertificateValidationConfigurationProvider))
                    path = AppSettingsConfigurationManager.GetSetting("securityConfigurationFile", String.Empty);
                if (t == typeof(JsonAuthorizationServerConfigurationManager))
                    path = AppSettingsConfigurationManager.GetSetting("authorizationServerConfiguration", String.Empty);
                if (String.IsNullOrWhiteSpace(path))
                    throw new NotSupportedException("Unsupported type");

                using (var reader = new StreamReader(path))
                {
                    return reader.ReadToEnd();
                }
            }, Lifetime.Singleton);
            return Task.CompletedTask;
        }
    }
}