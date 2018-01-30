using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kernel.Federation.MetaData;
using Kernel.Federation.MetaData.Configuration;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using Kernel.Federation.Protocols.Bindings.HttpRedirectBinding;
using Microsoft.Owin;
using Owin;
using Shared.Federtion.Factories;
using Shared.Federtion.Forms;
using Shared.Federtion.Request;

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
                    var metadataRequest = new MetadataGenerateRequest(MetadataType.Idp, "localIdp", new MetadataPublicationContext(c.Response.Body, MetadataPublicationProtocol.Http));
                    return metadataGenerator.CreateMetadata(metadataRequest);
                });

            });
            app.Map(new PathString("/api/sso/signon"), a =>
            {
                a.Run(async c =>
                {
                    var resolver = Kernel.Initialisation.ApplicationConfiguration.Instance.DependencyResolver;
                    var outboudContext = new HttpPostResponseOutboundContext(new SAMLForm()) { BindingContext = new BindingContext(new Dictionary<string, object>(), new Uri("http://localhost:60879/api/Account/SSOLogon")) };
                    outboudContext.DespatchDelegate = async form =>
                    {
                        await c.Response.WriteAsync(form.ToString());
                        //return Task.CompletedTask;
                    };
                    var protocolFactory = resolver.Resolve<Func<string, IProtocolHandler>>();
                    var protocolHanlder = protocolFactory(Bindings.Http_Post);
                    await protocolHanlder.HandleOutbound(new SamlProtocolContext { RequestContext = outboudContext });
                });
            });

            //owin middleware mock to parse auth request get the sp metadata and verrify the signarure
            //to be implementaed as OWIN middleware with handler and protocol handler.
            app.Map(new PathString("/sso/login"), a =>
            {
                a.Run(async c =>
                {
                    
                      //throw new NotImplementedException();
                    var elements = c.Request.Query;
                    var queryStringRaw = c.Request.QueryString.Value;

                    var resolver = Kernel.Initialisation.ApplicationConfiguration.Instance.DependencyResolver;
                    //var parser = resolver.Resolve<IMessageParser<SamlInboundContext, SamlInboundRequestContext>>();
                    var decoder = resolver.Resolve<IBindingDecoder<Uri>>();
                    var message = await decoder.Decode(c.Request.Uri);
                    var form = elements.ToDictionary(k => k.Key, v => v.Value.First());
                    var context = new HttpRedirectInboundContext
                    {
                        
                        Message = message,
                        HanlerAction = () =>
                        {
                            var id = Guid.NewGuid();
                            var urlBase = c.Request.Uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
                            c.Response.Redirect(String.Format("https://localhost:44342/client?returnUrl={0}{1}&state={2}", urlBase, "/api/sso/signon", id));
                        },
                        DescriptorResolver = m =>
                        {
                            var factory = resolver.Resolve<Func<Type, IMetadataHandler>>();
                            var metadataType = m.GetType();
                            var handlerType = typeof(IMetadataHandler<>).MakeGenericType(metadataType);
                            var handler = factory(handlerType);
                            if (handler == null)
                                throw new InvalidOperationException(String.Format("Handler must implement: {0}", typeof(IMetadataHandler).Name));
                            return handler.GetServiceProviderSingleSignOnDescriptor(m)
                            .Single()
                            .Roles.Single();
                        }
                    };
                    //await parser.Parse(context);
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