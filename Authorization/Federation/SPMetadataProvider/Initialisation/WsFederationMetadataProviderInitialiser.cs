using System;
using System.Threading.Tasks;
using Kernel.DependancyResolver;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.MetaData;
using Shared.Initialisation;
using WsFederationMetadataProvider.Metadata;

namespace WsFederationMetadataProvider.Initialisation
{
    public class WsFederationMetadataProviderInitialiser : Initialiser
    {
        public override byte Order
        {
            get { return 1; }
        }

        protected override Task InitialiseInternal(IDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterType<SPSSOMetadataProvider>(Lifetime.Transient);
            dependencyResolver.RegisterType<IdpSSOMetadataProvider>(Lifetime.Transient);
            
            dependencyResolver.RegisterFactory<Func<MetadataGenerateRequest, FederationPartyConfiguration>>(() => c =>
            {
                IFederationPartyContextBuilder builder;
                switch(c.MetadataType)
                {
                    case MetadataType.SP:
                        builder = dependencyResolver.Resolve<IAssertionPartyContextBuilder>();
                        break;
                    case MetadataType.Idp:
                        builder = dependencyResolver.Resolve<IRelyingPartyContextBuilder>();
                        break;
                    default:
                        throw new NotSupportedException(String.Format("Metadata type is not suported: {0}", c.MetadataType));
                }
                
                using (builder)
                {
                    return builder.BuildContext(c.FederationPartyId);
                }
            } , Lifetime.Singleton);
           
            return Task.CompletedTask;
        }
    }
}