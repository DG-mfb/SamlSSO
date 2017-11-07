using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Kernel.Federation.MetaData;
using Kernel.Federation.MetaData.Configuration;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpRedirectBinding;
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
                    var metadataRequest = new MetadataGenerateRequest(MetadataType.Idp, "localIdp", new MetadataPublishContext(c.Response.Body, MetadataPublishProtocol.Http));
                    return metadataGenerator.CreateMetadata(metadataRequest);
                });

            });
            app.Map(new PathString("/api/sso/signon"), a =>
            {
                a.Run(c =>
                {
                    return Task.CompletedTask;
                });

            });

            //owin middleware mock to parse auth request get the sp metadata and verrify the signarure
            //to be implementaed as OWIN middleware with handler and protocol handler.
            app.Map(new PathString("/sso/login"), a =>
            {
                a.Run(async c =>
                {
                    var elements = c.Request.Query;
                    var queryStringRaw = c.Request.QueryString.Value;

                    var resolver = Kernel.Initialisation.ApplicationConfiguration.Instance.DependencyResolver;
                    var context = new HttpRedirectInboundContext
                    {
                        Request = c.Request.QueryString.Value,
                        Form = elements.ToDictionary(k => k.Key, v => v.Value.First()),
                        HanlerAction = () =>
                        {
                            var id = Guid.NewGuid();
                            c.Response.Redirect(String.Format("https://localhost:44342/client?{0}{1}", "sso/login?", id));
                        }
                    };

                    var protocolFactory = resolver.Resolve<Func<string, IProtocolHandler>>();
                    var protocolHanlder = protocolFactory(Bindings.Http_Redirect);
                    await protocolHanlder.HandleInbound(new SamlProtocolContext { ResponseContext = context });
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