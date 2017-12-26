using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DeflateCompression;
using Federation.Protocols.Bindings.HttpRedirect;
using Federation.Protocols.Bindings.HttpRedirect.ClauseBuilders;
using Federation.Protocols.Encodiing;
using Federation.Protocols.RelayState;
using Federation.Protocols.Request;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.MetaData.Configuration.Cryptography;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpRedirectBinding;
using Kernel.Security.CertificateManagement;
using NUnit.Framework;
using SecurityManagement;
using Serialisation.JSON;
using Serialisation.JSON.SettingsProviders;
using Serialisation.Xml;
using Shared.Federtion.Constants;

namespace Federation.Protocols.Test.Request.Dispatchers
{
    [TestFixture]
    internal class RedirectRequestDispatcherTest
    {
        [Test]
        public async Task RedirectUrlBuildTest()
        {
            //ARRANGE
            var isValid = false;
            var builders = new List<IRedirectClauseBuilder>();

            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local");
            var spDescriptor = federationContex.MetadataContext.EntityDesriptorConfiguration.SPSSODescriptors.First();
            var certContext = spDescriptor.KeyDescriptors.Where(x => x.Use == KeyUsage.Signing && x.IsDefault)
                .Select(x => x.CertificateContext)
                .First();
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContex, supportedNameIdentifierFormats);
            authnRequestContext.RelyingState.Add("relayState", "Test state");
            var xmlSerialiser = new XMLSerialiser();
            var compressor = new DeflateCompressor();
            var encoder = new MessageEncoding(compressor);
            var logger = new LogProviderMock();
            var serialiser = new AuthnRequestSerialiser(xmlSerialiser, encoder, logger);
            AuthnRequestHelper.GetBuilders = AuthnRequestBuildersFactoryMock.GetBuildersFactory();
            var authnBuilder = new SamlRequestBuilder(serialiser);
            builders.Add(authnBuilder);
            
            //request compression builder
            var encodingBuilder = new RequestEncoderBuilder(encoder);
            builders.Add(encodingBuilder);

            //relay state builder
            var jsonSerialiser = new NSJsonSerializer(new DefaultSettingsProvider());
            var relayStateSerialiser = new RelaystateSerialiser(jsonSerialiser, encoder, logger) as IRelayStateSerialiser;
            var relayStateBuilder = new RelayStateBuilder(relayStateSerialiser);
            builders.Add(relayStateBuilder);

            //signature builder
            var certificateManager = new CertificateManager(logger);
            var signatureBuilder = new SignatureBuilder(certificateManager, logger);
            builders.Add(signatureBuilder);

            //context
            var outboundContext = new HttpRedirectRequestContext
            {
                BindingContext = new HttpRedirectContext(authnRequestContext),
                DespatchDelegate = redirectUri =>
                {
                    var query = redirectUri.Query.TrimStart('?');
                    var cert = certificateManager.GetCertificateFromContext(certContext);
                    isValid = this.VerifySignature(query, cert, certificateManager);
                    return Task.CompletedTask;
                }
            };
            //dispatcher
            var dispatcher = new RedirectRequestDispatcher(() => builders);

            //ACT
            await dispatcher.SendAsync(outboundContext);
            //ASSERT
            Assert.IsTrue(isValid);
        }
        private bool VerifySignature(string request, X509Certificate2 certificate, ICertificateManager certificateManager)
        {
            var i = request.IndexOf("Signature");
            var data = request.Substring(0, i - 1);
            var sgn = Uri.UnescapeDataString(request.Substring(i + 10));
            
            var validated = certificateManager.VerifySignatureFromBase64(data, sgn, certificate);
            return validated;
        }
    }
}