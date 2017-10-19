﻿using System;
using Kernel.DependancyResolver;
using Kernel.Federation.MetaData;
using Kernel.Initialisation;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Owin;
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
            SSOAuthenticationExtensions.RegisterLoggerFactory(app, resolver);
            app.Use((object)typeof(SSOOwinMiddleware), (object)app, (object)options, resolver);

            app.Map(options.SPMetadataPath, a =>
            {
                a.Run(c =>
                {
                    var federationParty = FederationPartyIdentifierHelper.GetFederationPartyIdFromRequestOrDefault(c);
                    var metadataGenerator = SSOAuthenticationExtensions.ResolveMetadataGenerator<ISPMetadataGenerator>();
                    c.Response.ContentType = "text/xml";
                    var metadataRequest = new MetadataGenerateRequest(MetadataType.SP, federationParty, new MetadataPublishContext(c.Response.Body, MetadataPublishProtocol.Http));
                    return metadataGenerator.CreateMetadata(metadataRequest);
                });
            });
            return app;
        }

        public static IAppBuilder UseSaml2SSOAuthentication(this IAppBuilder app, params string[] assertionEndpoints)
        {
            return SSOAuthenticationExtensions.UseSaml2SSOAuthentication(app, "/sp/metadata", assertionEndpoints);
        }

        public static IAppBuilder UseSaml2SSOAuthentication(this IAppBuilder app, string spMetadata, params string[] assertionEndpoints)
        {
            return SSOAuthenticationExtensions.UseSaml2SSOAuthentication(app, spMetadata, "/account/sso", assertionEndpoints);
        }

        public static IAppBuilder UseSaml2SSOAuthentication(this IAppBuilder app, string spMetadata, string ssoEndpoint, params string[] assertionEndpoints)
        {
            var options = new SSOAuthenticationOptions
            {
                SPMetadataPath = new PathString(spMetadata),
                SSOPath = new PathString(ssoEndpoint)
            };
            foreach (var s in assertionEndpoints)
            {
                options.AssertionEndPoinds.Add(new PathString(s));
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

        private static TMetadatGenerator ResolveMetadataGenerator<TMetadatGenerator>() where TMetadatGenerator : IMetadataGenerator
        {
            var resolver = ApplicationConfiguration.Instance.DependencyResolver;
            var metadataGenerator = resolver.Resolve<TMetadatGenerator>();
            return metadataGenerator;
        }
    }
}