using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Framework;
using SecurityManagement.Signing;
using System.Security.Cryptography.X509Certificates;
using Kernel.Federation.Constants;
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using SecurityManagement.Tests.Mock;
using System.IdentityModel.Metadata;

namespace SecurityManagement.Tests.Signing
{
    [TestFixture]
    internal class XmlSignatureManagerTest
    {
        [Test]
        [Ignore("File source.")]
        public void TempTest()
        {
            //ARRANGE
            var signatureManager = new XmlSignatureManager();
            var document = new XmlDocument() { PreserveWhitespace = false };
            document.Load(@"D:\Dan\Software\ECA-Interenational\Temp\TestResponse.xml");
            var signEl = document.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#")
                .Cast<XmlElement>()
                .First(x => x.ParentNode == document.DocumentElement);
            var certEl = (XmlElement)signEl.GetElementsByTagName("X509Certificate", "http://www.w3.org/2000/09/xmldsig#")[0];
            var dcert2 = new X509Certificate2(Convert.FromBase64String(certEl.InnerText));

            //ACT
            var isValid = signatureManager.VerifySignature(document, signEl, dcert2.PublicKey.Key);
            //ASSERT
            Assert.True(isValid);
        }

        [Test]
        [Ignore("File source")]
        public void TempAssertionTest()
        {
            //ARRANGE
            var foo = new StreamReader(@"D:\Dan\Software\ECA-Interenational\Temp\Base64.txt");
            var base64 = foo.ReadToEnd();
            var boo = Convert.FromBase64String(base64);
            var requestFromBase64 = Encoding.UTF8.GetString(boo);
            
            var signatureManager = new XmlSignatureManager();
            var document = new XmlDocument();
            //document.Load(@"D:\Dan\Software\ECA-Interenational\Temp\TestResponse.xml");
            var assertion = (XmlElement)document.GetElementsByTagName("Assertion", Saml20Constants.Assertion)[0];
            var signEl = document.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#")
                .Cast<XmlElement>()
                .First(x => x.ParentNode == assertion);
            var certEl = (XmlElement)signEl.GetElementsByTagName("X509Certificate", "http://www.w3.org/2000/09/xmldsig#")[0];
            var dcert2 = new X509Certificate2(Convert.FromBase64String(certEl.InnerText));
            var document1 = new XmlDocument();
            document1.LoadXml(assertion.OuterXml);
            var handler = new Saml2SecurityTokenHandler();
            var config = this.GetConfiguration();
            handler.Configuration = config;
            //var reader = XmlReader.Create(new StringReader(document.OuterXml));
            var reader = XmlReader.Create(new StringReader(requestFromBase64));
            //var reader = XmlReader.Create(@"D:\Dan\Software\ECA-Interenational\Temp\TestResponse.xml");
            this.MoveToToken(reader);
            //var reader1 = XmlReader.Create(new StringReader(document1.OuterXml));
            var token = handler.ReadToken(reader);
            //var token1 = handler.ReadToken(reader1);
            //ACT
            var isValid = signatureManager.VerifySignature(document1, signEl, dcert2.PublicKey.Key);
            var isValid1 = signatureManager.VerifySignature(document, signEl, dcert2.PublicKey.Key);
            //ASSERT
            Assert.True(isValid);
        }

        [Test]
        [Ignore("File source")]
        public void TempAssertionTest1()
        {
            //ARRANGE
            var foo = new StreamReader(@"D:\Dan\Software\ECA-Interenational\Temp\Base64.txt");
            var base64 = foo.ReadToEnd();
            var boo = Convert.FromBase64String(base64);
            var requestFromBase64 = Encoding.UTF8.GetString(boo);
            var reader = XmlReader.Create(new StringReader(requestFromBase64));
            var reader1 = XmlReader.Create(new StringReader(requestFromBase64));
            reader1.MoveToContent();
            var signatureManager = new XmlSignatureManager();
            var document = new XmlDocument { PreserveWhitespace = false };
            //document.Load(@"D:\Dan\Software\ECA-Interenational\Temp\TestResponse.xml");
            var xml = reader1.ReadOuterXml();
            document.LoadXml(xml);
            var assertion = (XmlElement)document.GetElementsByTagName("Assertion", Saml20Constants.Assertion)[0];
            var signEl = document.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#")
                .Cast<XmlElement>()
                .First(x => x.ParentNode == document.DocumentElement);
            var certEl = (XmlElement)signEl.GetElementsByTagName("X509Certificate", "http://www.w3.org/2000/09/xmldsig#")[0];
            var dcert2 = new X509Certificate2(Convert.FromBase64String(certEl.InnerText));
            //var document1 = new XmlDocument();
            //document1.LoadXml(assertion.OuterXml);
            var handler = new Saml2SecurityTokenHandler();
            var config = this.GetConfiguration();
            handler.Configuration = config;
            //var reader = XmlReader.Create(new StringReader(document.OuterXml));
            
            //var reader = XmlReader.Create(@"D:\Dan\Software\ECA-Interenational\Temp\TestResponse.xml", new XmlReaderSettings {  });
            this.MoveToToken(reader);
            //var reader1 = XmlReader.Create(new StringReader(document1.OuterXml));
            var token = handler.ReadToken(reader);
            //var token1 = handler.ReadToken(reader1);
            //ACT
            //var isValid = signatureManager.VerifySignature(document1, signEl, dcert2.PublicKey.Key);
            var isValid = signatureManager.VerifySignature(document, signEl, dcert2.PublicKey.Key);
            //ASSERT
            Assert.True(isValid);
        }

        [Test]
        [Ignore("File source")]
        public async Task VerifySugnature1()
        {
            var metadataPath = @"D:\Dan\Software\Apira\Temp\sso.flowz.co.uk007.xml";
            var path = @"D:\Dan\Software\Apira\Temp\XmlRequest.txt";
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
            var request = File.ReadAllText(path);

            var isValid = this.VerifySignature(request, cert);
            var isValid1 = this.VerifySignature(request, cert1);
            Assert.IsTrue(isValid);
            Assert.IsTrue(isValid1);
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
                    var signatureManager = new XmlSignatureManager();
                    var validated = signatureManager.VerifySignature(xmlDocument, certificate.PublicKey.Key);
                    return validated;
                }
            }
        }

        [Test]
        public async Task VerifyAuthRequestSignature()
        {
            //ARRANGE
            var signatureManager = new XmlSignatureManager();
            var document = new XmlDocument();
            var request = await SamlPostRequestProviderMock.BuildAuthnRequest();
            document.LoadXml(request.Item1);
            var cert = CertificateProviderMock.GetMockCertificate();

            //ACT
            
            var isValid = signatureManager.VerifySignature(document, cert.PublicKey.Key);
            
            //ASSERT
            Assert.True(isValid);
        }

        [Test]
        public void SignResponse_namespace_included()
        {
            //ARRANGE
            var signatureManager = new XmlSignatureManager();
            var inResponseTo = "Test_" + Guid.NewGuid();
            var tokenResponse = ResponseFactoryMock.GetTokenResponseSuccess(inResponseTo);
            var cert = CertificateProviderMock.GetMockCertificate();
            var serialised = ResponseFactoryMock.Serialize(tokenResponse);
            var document = new XmlDocument();
            document.LoadXml(serialised);

            //ACT
            signatureManager.WriteSignature(document, tokenResponse.ID, cert.PrivateKey, String.Empty, String.Empty, "saml,samlp");
            
            var isValid = signatureManager.VerifySignature(document, cert.PublicKey.Key);
            
            //ASSERT
            Assert.True(isValid);
        }

        [Test]
        public void SignResponse_namespace_not_included()
        {
            //ARRANGE
            var signatureManager = new XmlSignatureManager();
            var inResponseTo = "Test_" + Guid.NewGuid();
            var tokenResponse = ResponseFactoryMock.GetTokenResponseSuccess(inResponseTo);
            var cert = CertificateProviderMock.GetMockCertificate();
            var serialised = ResponseFactoryMock.Serialize(tokenResponse);
            var document = new XmlDocument();
            document.LoadXml(serialised);

            //ACT
            signatureManager.WriteSignature(document, tokenResponse.ID, cert.PrivateKey, String.Empty, String.Empty);

            var isValid = signatureManager.VerifySignature(document, cert.PublicKey.Key);

            //ASSERT
            Assert.True(isValid);
        }

        public SecurityTokenHandlerConfiguration GetConfiguration()
        {
            var inner = new X509CertificateStoreTokenResolver("testCertStore", StoreLocation.LocalMachine);
            var tokenResolver = new IssuerTokenResolver(inner);
            var configuration = new SecurityTokenHandlerConfiguration
            {
                IssuerTokenResolver = tokenResolver,
                ServiceTokenResolver = inner,
                AudienceRestriction = new AudienceRestriction(AudienceUriMode.Always),
                CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom,
            };
            
            return configuration;
        }

        internal void MoveToToken(XmlReader reader)
        {
            while (!(reader.IsStartElement(HttpRedirectBindingConstants.EncryptedAssertion, Saml20Constants.Assertion) || reader.IsStartElement("Assertion", Saml20Constants.Assertion)))
            {
                if (!reader.Read())
                    throw new InvalidOperationException("Can't find assertion element.");
            }
        }
    }
}