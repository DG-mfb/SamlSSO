﻿using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Federation.Protocols.Test.Mock;
using Federation.Protocols.Tokens;
using Kernel.Federation.Constants;
using NUnit.Framework;

namespace Federation.Protocols.Test.Tokens
{
    [TestFixture]
    internal class TokenHelperTests
    {
        [Test]
        public void GetPlainAsertion_Test()
        {
            //ARRANGE
            var doc = new XmlDocument();
            var path = FileHelper.GetEncryptedAssertionFilePath();
            doc.Load(path);
            var el = doc.DocumentElement;
            var inner = new X509CertificateStoreTokenResolver("TestCertStore", StoreLocation.LocalMachine);
            
            var encryptedList = el.GetElementsByTagName(HttpRedirectBindingConstants.EncryptedAssertion, Saml20Constants.Assertion);
            XmlDocument result = null;
            
            //ACT
            if (encryptedList.Count == 1)
            {
                var encryptedAssertion = (XmlElement)encryptedList[0];
                
                result = TokenHelper.GetPlainAsertion(inner, encryptedAssertion);
            }
            //ASSERT
            Assert.IsNotNull(result);
        }
    }
}