using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml;
using DeflateCompression;
using Federation.Protocols.Bindings.HttpPost;
using Federation.Protocols.Bindings.HttpPost.ClauseBuilders;
using Federation.Protocols.Encodiing;
using Federation.Protocols.RelayState;
using Federation.Protocols.Request;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.MetaData.Configuration.Cryptography;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using NUnit.Framework;
using SecurityManagement;
using SecurityManagement.Signing;
using Serialisation.JSON;
using Serialisation.JSON.SettingsProviders;
using Serialisation.Xml;
using Shared.Federtion.Constants;
using Shared.Federtion.Forms;

namespace Federation.Protocols.Test.Request.Dispatchers
{
    [TestFixture]
    internal class PostRequestDispatcherTest
    {
        [Test]
        public async Task Post_end_to_end_test()
        {
            //ARRANGE
            var isValid = false;
            string url = String.Empty;
            IDictionary<string, object> relayState = null;
            var builders = new List<IPostClauseBuilder>();

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
            
            //relay state builder
            var jsonSerialiser = new NSJsonSerializer(new DefaultSettingsProvider());
            var relayStateSerialiser = new RelaystateSerialiser(jsonSerialiser, encoder, logger) as IRelayStateSerialiser;
            var relayStateBuilder = new RelayStateBuilder(relayStateSerialiser);
            builders.Add(relayStateBuilder);

            //signature builder
            var certificateManager = new CertificateManager(logger);
            var xmlSinatureManager = new XmlSignatureManager();
            var signatureBuilder = new SignatureBuilder(certificateManager, logger, xmlSinatureManager);
            builders.Add(signatureBuilder);

            //context
            var outboundContext = new HttpPostRequestContext(new SAMLForm())
            {
                BindingContext = new RequestPostBindingContext(authnRequestContext),
                DespatchDelegate = form =>
                {
                    url = form.ActionURL;
                    var request = ((SAMLForm)form).HiddenControls[HttpRedirectBindingConstants.SamlRequest];
                    var state = ((SAMLForm)form).HiddenControls[HttpRedirectBindingConstants.RelayState];
                    var task = relayStateSerialiser.Deserialize(state);
                    task.Wait();
                    relayState = task.Result as IDictionary<string, object>;
                    var cert = certificateManager.GetCertificateFromContext(certContext);
                    isValid = this.VerifySignature(request, cert);
                    return Task.CompletedTask;
                }
            };

            //dispatcher
            var dispatcher = new PostRequestDispatcher(() => builders, logger);

            //ACT
            await dispatcher.SendAsync(outboundContext);
            //ASSERT
            Assert.AreEqual(url, requestUri.AbsoluteUri);
            Assert.IsTrue(Enumerable.SequenceEqual(relayState, authnRequestContext.RelyingState));
            Assert.IsTrue(isValid);
        }

        private bool VerifySignature(string request, X509Certificate2 certificate)
        {
            var unescaped = Uri.UnescapeDataString(request);
            var decoded = Convert.FromBase64String(unescaped);
            using (var ms = new MemoryStream(decoded))
            {
                ms.Position = 0;
                using (var streamReader = new StreamReader(ms))
                {
                    var xmlRequest = streamReader.ReadToEnd();
                    var xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(xmlRequest);
                    var signedXml = new SignedXml(xmlDocument);
                    XmlNodeList nodeList = xmlDocument.GetElementsByTagName("Signature");
                    signedXml.LoadXml((XmlElement)nodeList[0]);
                    var validated = signedXml.CheckSignature(certificate.PrivateKey);
                    return validated;
                }
            }
        }
    }
}