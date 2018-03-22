using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DeflateCompression;
using Federation.Protocols.Bindings.HttpRedirect;
using Federation.Protocols.Bindings.HttpRedirect.ClauseBuilders;
using Federation.Protocols.Encodiing;
using Federation.Protocols.RelayState;
using Federation.Protocols.Request;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.Constants;
using Kernel.Federation.MetaData.Configuration.Cryptography;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpRedirectBinding;
using Kernel.Federation.Protocols.Request;
using Kernel.Security.CertificateManagement;
using NUnit.Framework;
using SecurityManagement;
using Serialisation.JSON;
using Serialisation.JSON.SettingsProviders;
using Serialisation.Xml;

namespace Federation.Protocols.Test.Request.Dispatchers
{
    [TestFixture]
    internal class RedirectRequestDispatcherTest
    {
        [Test]
        public async Task Redirect_end_to_end_test()
        {
            //ARRANGE
            var isValid = false;
            string url = String.Empty;
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
            var serialiser = new RequestSerialiser(xmlSerialiser, encoder, logger);
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
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
                BindingContext = new RequestBindingContext(authnRequestContext),
                DespatchDelegate = redirectUri =>
                {
                    url = redirectUri.GetLeftPart(UriPartial.Path);
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
            Assert.AreEqual(url, requestUri.AbsoluteUri);
            Assert.IsTrue(isValid);
        }

        [Test]
        [Ignore("File source.")]
        public async Task VerifySugnature()
        {
            var uri = new Uri("https://sso.flowz.co.uk/sp/metadata");
            var path = @"D:\Dan\Software\Apira\Temp\RedirectRequest.txt";
            var logger = new LogProviderMock();
            var xmlReader = this.GetMetadataUrl(uri);
            var certificateManager = new CertificateManager(logger);
            var read = new MetadataSerializer
            {
                CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None
            };

            var meta = read.ReadMetadata(xmlReader);
            var descr = ((EntityDescriptor)meta).RoleDescriptors.First();
            var k = descr.Keys.Single(x => x.Use == KeyType.Signing);
            

                var kinfo = k.KeyInfo.First();

            var bi = kinfo as BinaryKeyIdentifierClause;
            var xr = kinfo as X509RawDataKeyIdentifierClause;
            var cert = new X509Certificate2(xr.GetX509RawData());
            var raw = bi.GetBuffer();
            var cert1 = new X509Certificate2(raw);
            //var key = kinfo.CreateKey() as X509AsymmetricSecurityKey;
            //var ak = key.GetAsymmetricAlgorithm(SignedXml.XmlDsigRSASHA1Url, false);
            var requestUrl = new Uri(File.ReadAllText(path));
            var url = requestUrl.GetLeftPart(UriPartial.Path);
            var query = requestUrl.Query.TrimStart('?');
            var isValid = this.VerifySignature(query, cert, certificateManager);
            var isValid1 = this.VerifySignature(query, cert1, certificateManager);
            Assert.IsTrue(isValid);
            Assert.IsTrue(isValid1);
        }

        [Test]
        [Ignore("File source.")]
        public async Task VerifySugnature1()
        {
            var metadataPath = @"D:\Dan\Software\Apira\Temp\sso.flowz.co.uk007.xml";
            var path = @"D:\Dan\Software\Apira\Temp\RedirectRequest.txt";
            var logger = new LogProviderMock();
            var xmlReader = XmlReader.Create(metadataPath);
            var certificateManager = new CertificateManager(logger);
            var read = new MetadataSerializer
            {
                CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None
            };

            var meta = read.ReadMetadata(xmlReader);
            var descr = ((EntityDescriptor)meta).RoleDescriptors.First();
            var k = descr.Keys.Single(x => x.Use == KeyType.Signing);


            var kinfo = k.KeyInfo.First();

            var bi = kinfo as BinaryKeyIdentifierClause;
            var xr = kinfo as X509RawDataKeyIdentifierClause;
            var cert = new X509Certificate2(xr.GetX509RawData());
            var raw = bi.GetBuffer();
            var cert1 = new X509Certificate2(raw);
            //var key = kinfo.CreateKey() as X509AsymmetricSecurityKey;
            //var ak = key.GetAsymmetricAlgorithm(SignedXml.XmlDsigRSASHA1Url, false);
            var requestUrl = new Uri(File.ReadAllText(path));
            var url = requestUrl.GetLeftPart(UriPartial.Path);
            var query = requestUrl.Query.TrimStart('?');
            var isValid = this.VerifySignature(query, cert, certificateManager);
            var isValid1 = this.VerifySignature(query, cert1, certificateManager);
            Assert.IsTrue(isValid);
            Assert.IsTrue(isValid1);
        }

        private XmlReader GetMetadataUrl(Uri url)
        {
            var request = System.Net.WebRequest.Create(url);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback((_, __, ____, ___) => true);

            var ms = new MemoryStream();

            var sb = new StringBuilder();
            using (var response = request.GetResponse().GetResponseStream())
            {
                response.CopyTo(ms);
                response.Close();
            }
            ms.Seek(0, SeekOrigin.Begin); // Rewind memorystream back to the beginning

            XmlReader reader = XmlReader.Create(new StreamReader(ms));

            return reader;

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