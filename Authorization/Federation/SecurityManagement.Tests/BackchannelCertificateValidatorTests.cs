using System;
using NUnit.Framework;

namespace SecurityManagement.Tests
{
    [TestFixture]
    [Ignore("Not implemented")]
    public class BackchannelCertificateValidatorTests
    {
        [Test]
        public void RemoteCertificateValidationRulesTest()
        {
            throw new NotImplementedException();
            //ARRANGE
            //var configuration = new BackchannelConfiguration
            //{
            //    UsePinningValidation = false
            //};
            //var configurationProvider = new CertificateValidationConfigurationProvider(() => configuration);
            //var logger = new LogProviderMock();
            //var validator = new BackchannelCertificateValidator(configurationProvider, logger);

            //var certificateStore = new X509Store("TestCertStore", StoreLocation.LocalMachine);
            //var validationResult = false;
            ////ACT
            //try
            //{
            //    certificateStore.Open(OpenFlags.ReadOnly);
            //    var certificate = certificateStore.Certificates.Find(X509FindType.FindBySubjectName, "ApiraTestCertificate", false)[0];
            //    var x509Chain = new X509Chain(true);
            //    x509Chain.Build(certificate);
            //    validationResult = validator.Validate(HttpWebRequest.Create(new Uri("http://localhost:60879/")), certificate, x509Chain, SslPolicyErrors.None);
            //}
            //finally
            //{
            //    certificateStore.Close();
            //    certificateStore.Dispose();
            //}
            ////ASSERT
            //Assert.True(validationResult);
        }
    }
}