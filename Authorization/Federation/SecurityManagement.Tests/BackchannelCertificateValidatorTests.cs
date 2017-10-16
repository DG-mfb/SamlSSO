﻿using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Kernel.Cryptography.Validation;
using NUnit.Framework;
using SecurityManagement.Tests.Mock;

namespace SecurityManagement.Tests
{
    [TestFixture]
    
    public class BackchannelCertificateValidatorTests
    {
        [Test]
        public void RemoteCertificateValidationRulesTest()
        {
            //ARRANGE
            var configuration = new CertificateValidationConfiguration
            {
                UsePinningValidation = false,
                X509CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom
            };
            var configurationProvider = new CertificateValidationConfigurationProvider(() => configuration);
            var logger = new LogProviderMock();
            var validator = new BackchannelCertificateValidator(configurationProvider, logger);

            var certificateStore = new X509Store("TestCertStore", StoreLocation.LocalMachine);
            var validationResult = false;
            //ACT
            try
            {
                certificateStore.Open(OpenFlags.ReadOnly);
                var certificate = certificateStore.Certificates.Find(X509FindType.FindBySubjectName, "ApiraTestCertificate", false)[0];
                var x509Chain = new X509Chain(true);
                x509Chain.Build(certificate);
                validationResult = validator.Validate(HttpWebRequest.Create(new Uri("http://localhost:60879/")), certificate, x509Chain, SslPolicyErrors.None);
            }
            finally
            {
                certificateStore.Close();
                certificateStore.Dispose();
            }
            //ASSERT
            Assert.True(validationResult);
        }
    }
}