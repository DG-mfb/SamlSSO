using System;
using System.Linq;
using System.Threading.Tasks;
using Kernel.DependancyResolver;
using Kernel.Extensions;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.MetaData;
using Kernel.Initialisation;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Owin;
using SSOOwinMiddleware.Discovery;
using SSOOwinMiddleware.Logging;

namespace SSOOwinMiddleware.Extensions
{
    public static class SSOAuthenticationExtensions
    {
        public static IAppBuilder UseSaml2SSOAuthentication(this IAppBuilder app, SSOAuthenticationOptions options)
        {
            if (app == null)
                throw new ArgumentNullException("app");
            if (options == null)
                throw new ArgumentNullException("options");
            var resolver = ApplicationConfiguration.Instance.DependencyResolver;
            
            app.Use((object)typeof(SSOOwinMiddleware), (object)options, resolver);
            
            return app;
        }

        public static IAppBuilder UseMetadataMiddleware(this IAppBuilder app, string metadataPath, MetadataType metadataType, IDependencyResolver resolver)
        {
            app.Map(new PathString(metadataPath), a =>
            {
                a.Run(c =>
                {
                    var discoveryService = resolver.Resolve<IDiscoveryService<IOwinContext, string>>();
                    var federationParty = discoveryService.ResolveParnerId(c);
                    IMetadataGenerator metadataGenerator;
                    switch(metadataType)
                    {
                        case MetadataType.SP:
                            metadataGenerator = resolver.Resolve<ISPMetadataGenerator>();
                            break;
                        case MetadataType.Idp:
                            metadataGenerator = resolver.Resolve<IIdPMetadataGenerator>();
                            break;
                        default:
                            throw new NotSupportedException(String.Format(" Not supported metadata type: {0}", metadataType));
                    }
                    
                    c.Response.ContentType = "text/xml";
                    var metadataRequest = new MetadataGenerateRequest(metadataType, federationParty, new MetadataPublicationContext(c.Response.Body, MetadataPublicationProtocol.Http));
                    return metadataGenerator.CreateMetadata(metadataRequest);
                });
            });
            app.Map(new PathString("/metadata/requestRefresh"), a =>
            {
                a.Run(c =>
                {
                    var discoveryService = resolver.Resolve<IDiscoveryService<IOwinContext, string>>();
                    var federationParty = discoveryService.ResolveParnerId(c);
                    var configurationManager = (IConfigurationManager<FederationPartyConfiguration>)resolver.Resolve<IAssertionPartyContextBuilder>();
                    configurationManager.RequestRefresh(federationParty);
                    return Task.CompletedTask;
                });
            });
            return app;
        }
        
        public static IAppBuilder UseSaml2SSOAuthentication(this IAppBuilder app, params string[] assertionEndpoints)
        {
            return SSOAuthenticationExtensions.UseSaml2SSOAuthentication(app, "/account/sso", assertionEndpoints);
        }

        public static IAppBuilder UseSaml2SSOAuthentication(this IAppBuilder app, string ssoEndpoint, params string[] assertionEndpoints)
        {
            var options = new SSOAuthenticationOptions
            {
                SSOPath = new PathString(ssoEndpoint)
            };
            foreach (var s in assertionEndpoints)
            {
                options.AssertionEndpoinds.Add(new PathString(s));
            }
            return app.UseSaml2SSOAuthentication(options);
        }

        public static IAppBuilder RegisterLoggerFactory(this IAppBuilder app, IDependencyResolver resolver)
        {
            var owinLoggerFactory = AppBuilderLoggerExtensions.GetLoggerFactory(app);
            var loggerFactory = new CustomLoggerFactory(resolver, owinLoggerFactory);
            AppBuilderLoggerExtensions.SetLoggerFactory(app, loggerFactory);
            return app;
        }

        public static IAppBuilder RegisterLogger(this IAppBuilder app, IDependencyResolver resolver)
        {
            SSOAuthenticationExtensions.RegisterLoggerFactory(app, resolver);
            resolver.RegisterFactory<ILogger>(() => app.CreateLogger<SSOOwinMiddleware>(), Lifetime.Transient);
            return app;
        }

        public static IAppBuilder RegisterDiscoveryService(this IAppBuilder app, IDependencyResolver resolver)
        {
            return SSOAuthenticationExtensions.RegisterDiscoveryService(app, resolver, typeof(DiscoveryService));
        }

        public static IAppBuilder RegisterDiscoveryService(this IAppBuilder app, IDependencyResolver resolver, Type discoveryService)
        {
            if (!TypeExtensions.IsAssignableToGenericType(discoveryService, typeof(IDiscoveryService<IOwinContext, string>)))
                throw new InvalidOperationException(String.Format("Discovery service must implement interface: IDiscoveryService<TContext, TResult>"));
            
            resolver.RegisterType(discoveryService, Lifetime.Singleton);
            return app;
        }
    }
}