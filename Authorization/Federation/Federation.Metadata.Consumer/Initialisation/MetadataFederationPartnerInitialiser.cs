using System;
using System.IdentityModel.Metadata;
using System.Threading.Tasks;
using Federation.Metadata.FederationPartner.Configuration;
using Federation.Metadata.FederationPartner.Handlers;
using Kernel.DependancyResolver;
using Kernel.Web;
using Shared.Federtion;
using Shared.Initialisation;

namespace Federation.Metadata.FederationPartner.Initialisation
{
    public class MetadataFederationPartnerInitialiser : Initialiser
    {
        public override byte Order
        {
            get { return 1; }
        }

        protected override Task InitialiseInternal(IDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterType<WsFederationConfigurationRetriever>(Lifetime.Transient);
            dependencyResolver.RegisterType<FederationConfigurationManager>(Lifetime.Singleton);
            dependencyResolver.RegisterType<MetadataEntitiesDescriptorHandler>(Lifetime.Transient);
            dependencyResolver.RegisterType<MetadataEntitityDescriptorHandler>(Lifetime.Transient);
            dependencyResolver.RegisterFactory<Func<string, IDocumentRetriever>>(_ =>
                {
                    return s => 
                    {
                        if (String.IsNullOrWhiteSpace(s))
                            throw new ArgumentNullException("path");
                        if(s.StartsWith("file", StringComparison.OrdinalIgnoreCase))
                            return dependencyResolver.Resolve<IFileDocumentRetriever>();
                        if (s.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                            return dependencyResolver.Resolve<IHttpDocumentRetriever>();
                        throw new NotSupportedException(String.Format("Not supported path schema{0}. Supported schemas: http, file://", s));
                    };
                }, Lifetime.Singleton);
            dependencyResolver.RegisterFactory<Action<MetadataBase>>(() => m => 
            {
                IdentityConfigurationHelper.OnReceived(m, dependencyResolver);

            }, Lifetime.Singleton);
            return Task.CompletedTask;
        }
    }
}