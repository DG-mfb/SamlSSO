using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Shared.Federtion.Constants;

namespace Federation.Protocols.Test.Mock
{
    internal class AssertionFactroryMock
    {
        public static Saml2Assertion BuildAssertion()
        {
            var assertion = new Saml2Assertion(new Saml2NameIdentifier("https://dg-mfb/idp/shibboleth", new Uri(NameIdentifierFormats.Entity)));
            assertion.Subject = new Saml2Subject(new Saml2NameIdentifier("TestSubject", new Uri(NameIdentifierFormats.Persistent)));
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
    }
}