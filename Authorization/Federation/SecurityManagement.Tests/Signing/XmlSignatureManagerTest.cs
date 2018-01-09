using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace SecurityManagement.Tests.Signing
{
    [TestFixture]
    internal class XmlSignatureManagerTest
    {
        [Test]
        [Ignore("Temp")]
        public void TempTest()
        {
            //ARRANGE
            var signatureManager = new XmlSignatureManager();
            var document = new XmlDocument();
            document.Load(@"D:\Dan\Software\ECA-Interenational\Temp\AtlasResponse.xml");
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
        [Ignore("Temp test")]
        public void TempAssertionTest()
        {
            //ARRANGE
            var signatureManager = new XmlSignatureManager();
            var document = new XmlDocument();
            document.Load(@"D:\Dan\Software\ECA-Interenational\Temp\AtlasResponse.xml");
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
            var reader = XmlReader.Create(new StringReader(document.OuterXml));
            this.MoveToToken(reader);
            var reader1 = XmlReader.Create(new StringReader(document1.OuterXml));
            var token = handler.ReadToken(reader);
            var token1 = handler.ReadToken(reader1);
            //ACT
            var isValid = signatureManager.VerifySignature(document1, signEl, dcert2.PublicKey.Key);
            var isValid1 = signatureManager.VerifySignature(document, signEl, dcert2.PublicKey.Key);
            //ASSERT
            Assert.True(isValid);
        }

        [Test]
        public void SignRequest()
        {
            //ARRANGE
            var signatureManager = new XmlSignatureManager();
            var document = new XmlDocument();
            document.Load(@"D:\Dan\Software\ECA-Interenational\Temp\AuthnRequest.xml");
            var cert = CertificateProviderMock.GetMockCertificate();

            //ACT
            signatureManager.SignXml(document, "eca_e2376d50-2f84-4a6b-bdc6-8283a8b2d990_bde559c1-fe68-4ff8-898f-5660a794156a", cert.PrivateKey, "saml,samlp");

            var signEl = document.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#")
                .Cast<XmlElement>()
                .First(x => x.ParentNode == document.DocumentElement);
            var isValid = signatureManager.VerifySignature(document, signEl, cert.PublicKey.Key);
            //ASSERT
            Assert.True(isValid);
        }

        [Test]
        public void SignResponse()
        {
            //ARRANGE
            var signatureManager = new XmlSignatureManager();
            var document = new XmlDocument();
            document.Load(@"D:\Dan\Software\ECA-Interenational\Temp\TestResponse.xml");
            var cert = CertificateProviderMock.GetMockCertificate();

            //ACT
            signatureManager.SignXml(document, "Test_64f68a98-05fc-4e68-a0e0-ed7edb76c8df", cert.PrivateKey, "saml,samlp");

            var signEl = document.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#")
                .Cast<XmlElement>()
                .First(x => x.ParentNode == document.DocumentElement);
            var isValid = signatureManager.VerifySignature(document, signEl, cert.PublicKey.Key);
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
