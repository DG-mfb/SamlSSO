using System.Threading.Tasks;
using Kernel.Federation.MetaData;
using Kernel.Federation.Protocols;
using Microsoft.Owin;
using Owin;
using System.Linq;
using Shared.Federtion.Models;

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
                    var metadataRequest = new MetadataGenerateRequest(MetadataType.Idp, "localIdp", new MetadataPublishContext(c.Response.Body, MetadataPublishProtocol.Http));
                    return metadataGenerator.CreateMetadata(metadataRequest);
                });

            });

            app.Map(new PathString("/sso/login"), a =>
            {
                a.Run(async c =>
                {
                    var resolver = Kernel.Initialisation.ApplicationConfiguration.Instance.DependencyResolver;
                    var relayStateHandler = resolver.Resolve<IRelayStateHandler>();
                    var authnRequestSerialiser = resolver.Resolve<IAuthnRequestSerialiser>();
                    var elements = c.Request.Query;
                    var requestEncoded = elements["SAMLRequest"];
                    var relayState = await relayStateHandler.GetRelayStateFromFormData(elements.ToDictionary(k => k.Key, v => v.Value.First()));
                    var request = await authnRequestSerialiser.Deserialize<AuthnRequest>(requestEncoded);
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