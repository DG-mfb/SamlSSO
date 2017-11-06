using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.MetaData;
using Kernel.Federation.Protocols;
using Kernel.Security.CertificateManagement;
using Microsoft.Owin;
using Owin;
using Shared.Federtion.Factories;
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
                    var queryStringRaw = c.Request.QueryString.Value;
                   
                    var requestEncoded = elements["SAMLRequest"];
                    var relayState = await relayStateHandler.GetRelayStateFromFormData(elements.ToDictionary(k => k.Key, v => v.Value.First()));
                    var request = await authnRequestSerialiser.Deserialize<AuthnRequest>(requestEncoded);
                    var configManager = resolver.Resolve<IConfigurationRetriever<MetadataBase>>();
                    var spMetadata = await configManager.GetAsync(new FederationPartyConfiguration("local", "http://localhost:60879/sp/metadata"), CancellationToken.None);
                    var metadataType = spMetadata.GetType();
                    var handlerType = typeof(IMetadataHandler<>).MakeGenericType(metadataType);
                    var handler = resolver.Resolve(handlerType) as IMetadataHandler<EntityDescriptor>;
                    if (handler == null)
                        throw new InvalidOperationException(String.Format("Handler must implement: {0}", typeof(IMetadataHandler).Name));
                    var sp = handler.GetRoleDescriptors<ServiceProviderSingleSignOnDescriptor>((EntityDescriptor)spMetadata)
                        .Single();
                    var keyDescriptors = sp.Keys.Where(k => k.Use == KeyType.Signing);
                    var validated = false;
                    foreach (var k in keyDescriptors.SelectMany(x => x.KeyInfo))
                    {
                        var binaryClause = k as BinaryKeyIdentifierClause;
                        if (binaryClause == null)
                            throw new InvalidOperationException(String.Format("Expected type: {0} but it was: {1}", typeof(BinaryKeyIdentifierClause), k.GetType()));

                        var certContent = binaryClause.GetBuffer();
                        var cert = new X509Certificate2(certContent);
                        validated = this.VerifySignature(queryStringRaw, cert, certificateManager);
                        if (validated)
                            break;
                    }
                    if (!validated)
                        throw new InvalidOperationException("Invalid signature.");
                    var id = Guid.NewGuid();
                    c.Response.Redirect(String.Format("https://localhost:44342/client?{0}{1}", "https://localhost:44342/account/sso/login/", id));
                });
            });
        }

        private bool VerifySignature(string request, X509Certificate2 certificate, ICertificateManager certificateManager)
        {
            var i = request.IndexOf("Signature");
            var data = request.Substring(0, i - 1);
            var sgn = Uri.UnescapeDataString(request.Substring(i + 10));
           

            var validated = certificateManager.VerifySignatureFromBase64(data, sgn, certificate);
            return validated;
        }
        private static TMetadatGenerator ResolveMetadataGenerator<TMetadatGenerator>() where TMetadatGenerator : IMetadataGenerator
        {
            var resolver = Kernel.Initialisation.ApplicationConfiguration.Instance.DependencyResolver;
            var metadataGenerator = resolver.Resolve<TMetadatGenerator>();
            return metadataGenerator;
        }
    }
}