using System;
using Kernel.DependancyResolver;
using Kernel.Extensions;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.MetaData;
using Kernel.Initialisation;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Owin;
using SLOOwinMiddleware.Logging;

namespace SLOOwinMiddleware.Extensions
{
    public static class SLOAuthenticationExtensions
    {
        public static IAppBuilder UseSaml2SLOAuthentication(this IAppBuilder app, SLOAuthenticationOptions options)
        {
            if (app == null)
                throw new ArgumentNullException("app");
            if (options == null)
                throw new ArgumentNullException("options");
            var resolver = ApplicationConfiguration.Instance.DependencyResolver;
            
            app.Use((object)typeof(SLOOwinMiddleware), (object)options, resolver);
            
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
            return app;
        }

        public static IAppBuilder UseSaml2SLOAuthentication(this IAppBuilder app)
        {
            var options = new SLOAuthenticationOptions();
           
            return app.UseSaml2SLOAuthentication(options);
        }

        public static IAppBuilder RegisterLoggerFactory(this IAppBuilder app, IDependencyResolver resolver)
        {
            var owinLoggerFactory = AppBuilderLoggerExtensions.GetLoggerFactory(app);
            var loggerFactory = new CustomLoggerFactory(resolver, owinLoggerFactory);
            AppBuilderLoggerExtensions.SetLoggerFactory(app, loggerFactory);
            return app;
        }

        //public static IAppBuilder RegisterLogger(this IAppBuilder app, IDependencyResolver resolver)
        //{
        //    SLOAuthenticationExtensions.RegisterLoggerFactory(app, resolver);
        //    resolver.RegisterFactory<ILogger>(() => app.CreateLogger<SLOOwinMiddleware>(), Lifetime.Transient);
        //    return app;
        //}

        //public static IAppBuilder RegisterDiscoveryService(this IAppBuilder app, IDependencyResolver resolver)
        //{
        //    return SSOAuthenticationExtensions.RegisterDiscoveryService(app, resolver, typeof(DiscoveryService));
        //}

        public static IAppBuilder RegisterDiscoveryService(this IAppBuilder app, IDependencyResolver resolver, Type discoveryService)
        {
            if (!TypeExtensions.IsAssignableToGenericType(discoveryService, typeof(IDiscoveryService<IOwinContext, string>)))
                throw new InvalidOperationException(String.Format("Discovery service must impelemnt interface: IDiscoveryService<TContext, TResult>"));
            
            resolver.RegisterType(discoveryService, Lifetime.Singleton);
            return app;
        }
    }
}