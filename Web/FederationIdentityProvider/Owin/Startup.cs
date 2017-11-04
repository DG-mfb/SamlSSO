using Kernel.Federation.MetaData;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FederationIdentityProvider.Owin.Startup))]

namespace FederationIdentityProvider.Owin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map(new PathString("/idp/metadata"), a =>
            {
                a.Run(c =>
                {
                    var metadataGenerator = Startup.ResolveMetadataGenerator<IIdPMetadataGenerator>();
                    c.Response.ContentType = "text/xml";
                    var metadataRequest = new MetadataGenerateRequest(MetadataType.SP, "localIdp", new MetadataPublishContext(c.Response.Body, MetadataPublishProtocol.Http));
                    return metadataGenerator.CreateMetadata(metadataRequest);
                });

            });
        }

        private static TMetadatGenerator ResolveMetadataGenerator<TMetadatGenerator>() where TMetadatGenerator : IMetadataGenerator
        {
            var resolver = Kernel.Initialisation.ApplicationConfiguration.Instance.DependencyResolver;
            var metadataGenerator = resolver.Resolve<TMetadatGenerator>();
            return metadataGenerator;
        }
    }
}