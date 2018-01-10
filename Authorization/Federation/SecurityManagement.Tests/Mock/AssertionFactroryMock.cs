using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using Kernel.Federation.Constants;

namespace SecurityManagement.Tests.Mock
{
    internal class AssertionFactroryMock
    {
        private static X509Certificate2 mockCert;

        public static Saml2Assertion BuildAssertion()
        {
            var certificate = AssertionFactroryMock.GetMockCertificate();
            var signingCredentials = new SigningCredentials(new X509AsymmetricSecurityKey(certificate), SecurityAlgorithms.RsaSha1Signature, SecurityAlgorithms.Sha1Digest, new SecurityKeyIdentifier(new X509RawDataKeyIdentifierClause(certificate)));
            var assertion = new Saml2Assertion(new Saml2NameIdentifier("https://dg-mfb/idp/shibboleth", new Uri(NameIdentifierFormats.Entity)));
            assertion.Subject = new Saml2Subject(new Saml2NameIdentifier("TestSubject", new Uri(NameIdentifierFormats.Persistent)));
            assertion.SigningCredentials = signingCredentials;
            return assertion;
        }

        public static Saml2SecurityToken GetToken(Saml2Assertion assertion)
        {
            var token = new Saml2SecurityToken(assertion);
            return token;
        }

        public static XmlElement SerialiseToken(Saml2SecurityToken token)
        {
            var handler = new Saml2SecurityTokenHandler();
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                handler.WriteToken(writer, token);
                writer.Flush();
                var document = new XmlDocument();
                document.LoadXml(sb.ToString());
                return document.DocumentElement;
            }
        }
        public static X509Certificate2 GetMockCertificate()
        {
            if (AssertionFactroryMock.mockCert == null)
            {
                using (var store = new X509Store("testCertStore", StoreLocation.LocalMachine))
                {
                    try
                    {
                        store.Open(OpenFlags.ReadOnly| OpenFlags.OpenExistingOnly);
                        var certSource = store.Certificates.Find(X509FindType.FindBySubjectName, "www.eca-international.com", false)[0];
                        AssertionFactroryMock.mockCert = certSource;
                    }
                    finally
                    {
                        store.Close();
                    }
                }
            }
            return AssertionFactroryMock.mockCert;
        }
    }
}