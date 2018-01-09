using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Kernel.Federation.Constants;
using Serialisation.Xml;
using Shared.Federtion.Models;
using Shared.Federtion.Response;

namespace Federation.Protocols.Test.Mock
{
    internal class ResponseFactoryMock
    {
        public static TokenResponse GetTokenResponseSuccess(string inResponseTo, string statusCode)
        {
            var response = ResponseFactoryMock.GetTokenResponse(inResponseTo, statusCode);
            var assertion = AssertionFactroryMock.BuildAssertion();
            var token = AssertionFactroryMock.GetToken(assertion);
            var assertionElement = AssertionFactroryMock.SerialiseToken(token);
            response.Assertions = new XmlElement[] { assertionElement };
            return response;
        }

        public static TokenResponse GetTokenResponse(string inResponseTo, string statusCode)
        {
            var response = new TokenResponse
            {
                ID = "Test_" + Guid.NewGuid().ToString(),
                Destination = "http://localhost:59611/",
                IssueInstant = DateTime.UtcNow,
                InResponseTo = inResponseTo,
                Status = ResponseFactoryMock.BuildStatus(statusCode, null),
                Issuer = new NameId { Value = "https://dg-mfb/idp/shibboleth", Format = NameIdentifierFormats.Entity }
            };
            return response;
        }

        public static LogoutResponse GetLogoutResponse(string inResponseTo, string statusCode)
        {
            var response = new LogoutResponse
            {
                ID = "Test_" + Guid.NewGuid().ToString(),
                Destination = "http://localhost:59611/",
                IssueInstant = DateTime.UtcNow,
                InResponseTo = inResponseTo,
                Status = ResponseFactoryMock.BuildStatus(statusCode, null),
                Issuer = new NameId { Value = "https://dg-mfb/idp/shibboleth", Format = NameIdentifierFormats.Entity }
            };
            return response;
        }

        public static Status BuildStatus(string code, string message = null)
        {
            return new Status
            {
                StatusMessage = message,
                StatusCode = ResponseFactoryMock.GetStatusCode(code, null)
            };
        }

        public static void BuildStatuseDetail(Status status, ICollection<XmlElement> details)
        {
            status.StatusDetail = new StatusDetail
            {
                Any = details.ToArray()
            };
        }

        public static StatusCode GetStatusCode(string code, StatusCode parent)
        {
            var statusCode = new StatusCode
            {
                Value = code
            };
            if (parent != null)
            {
                parent.SubStatusCode = statusCode;
                return parent;
            }
            return statusCode;
        }

        public static string Serialize(object o)
        {
            var xmlSerialiser = new XMLSerialiser();
            xmlSerialiser.XmlNamespaces.Add("samlp", Saml20Constants.Protocol);
            xmlSerialiser.XmlNamespaces.Add("saml", Saml20Constants.Assertion);

            using (var ms = new MemoryStream())
            {
                xmlSerialiser.Serialize(ms, new[] { o });
                ms.Position = 0;
                var streamReader = new StreamReader(ms);
                var xmlString = streamReader.ReadToEnd();
                return xmlString;
            }
        }
    }
}