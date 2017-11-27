using System;
using System.Security.Cryptography.X509Certificates;
using Kernel.Security.CertificateManagement;
using NUnit.Framework;
using SecurityManagement.Tests.Mock;

namespace SecurityManagement.Tests.Manager
{
    [TestFixture]
    internal class CertificateManagerTests
    {
        [Test]
        public void SignDataToBase64Test()
        {
            //ARRANGE
            var data = "Data to sign";
            var logger = new LogProviderMock();
            var manager = new CertificateManager(logger);
            var certContext = new X509CertificateContext
            {
                StoreLocation = StoreLocation.LocalMachine,
                ValidOnly = false,
                StoreName = "TestCertStore"
            };
            certContext.SearchCriteria.Add(new CertificateSearchCriteria
            {
                SearchCriteriaType = X509FindType.FindBySubjectName,
                SearchValue = "www.eca-international.com"
            });
            //ACT
            var signed = manager.SignToBase64(data, certContext);
            var verified = manager.VerifySignatureFromBase64(data, signed, certContext);
            //ASSERT
            Assert.IsTrue(verified);
        }

        [Test]
        public void SignDataTest()
        {
            //ARRANGE
            var data = "Data to sign";
            var logger = new LogProviderMock();
            var manager = new CertificateManager(logger);
            var certContext = new X509CertificateContext
            {
                StoreLocation = StoreLocation.LocalMachine,
                ValidOnly = false,
                StoreName = "TestCertStore"
            };
            certContext.SearchCriteria.Add(new CertificateSearchCriteria
            {
                SearchCriteriaType = X509FindType.FindBySubjectName,
                SearchValue = "www.eca-international.com"
            });
            //ACT
            var cert = manager.GetCertificateFromContext(certContext);
            var signed = manager.SignData(data, cert);
            var signedBytes = Convert.ToBase64String(signed);
            var verified = manager.VerifySignatureFromBase64(data, signedBytes, certContext);
            //ASSERT
            Assert.IsTrue(verified);
        }
    }
}