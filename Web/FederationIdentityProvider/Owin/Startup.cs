using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Kernel.Federation.MetaData;
using Kernel.Federation.Protocols;
using Kernel.Security.CertificateManagement;
using Microsoft.Owin;
using Owin;
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
            app.Map(new PathString("/account/sso/login"), a =>
            {
                a.Run(c =>
                {
                    return Task.CompletedTask;
                });

            });
            app.Map(new PathString("/sso/login"), a =>
            {
                a.Run(async c =>
                {
                    var resolver = Kernel.Initialisation.ApplicationConfiguration.Instance.DependencyResolver;
                    var certificateManager = resolver.Resolve<ICertificateManager>();
                    var relayStateHandler = resolver.Resolve<IRelayStateHandler>();
                    var authnRequestSerialiser = resolver.Resolve<IAuthnRequestSerialiser>();
                    var elements = c.Request.Query;
                    var qs = c.Request.QueryString.Value;
                    var i = qs.IndexOf("Signature");
                    var data = qs.Substring(0, i - 1);
                    var sgn = Uri.UnescapeDataString(qs.Substring(i + 10));
                    var certContext = new X509CertificateContext
                    {
                        StoreLocation = StoreLocation.LocalMachine,
                        ValidOnly = false,
                        StoreName = "TestCertStore"
                    };
                    certContext.SearchCriteria.Add(new CertificateSearchCriteria
                    {
                        SearchCriteriaType = X509FindType.FindBySubjectName,
                        SearchValue = "ApiraTestCertificate"
                    });
                    
                    var validated = certificateManager.VerifySignatureFromBase64(data, sgn, certContext);
                    if (!validated)
                        throw new InvalidOperationException("Invalid signature.");

                    var requestEncoded = elements["SAMLRequest"];
                    var relayState = await relayStateHandler.GetRelayStateFromFormData(elements.ToDictionary(k => k.Key, v => v.Value.First()));
                    var request = await authnRequestSerialiser.Deserialize<AuthnRequest>(requestEncoded);
                    var id = Guid.NewGuid();
                    c.Response.Redirect(String.Format("https://localhost:44342/client/src?id={0}", id));
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