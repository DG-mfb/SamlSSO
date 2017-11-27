using System;
using System.Security.Cryptography.X509Certificates;
using Kernel.Security.Configuration;
using Kernel.Security.Validation;
using NUnit.Framework;
using SecurityManagement.Tests.Mock;

namespace SecurityManagement.Tests
{
    [TestFixture]
    
    public class CertificateValidatorTests
    {
        [Test]  
        public void MetadataSerialisationCertificateTest_success()
        {
            //ARRANGE
            var logger = new LogProviderMock();
            var store = new X509Store("TestCertStore");
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var certificate = store.Certificates.Find(X509FindType.FindBySubjectName, "www.eca-international.com", false)[0];
                var configuration = new CertificateValidationConfiguration
                {
                    X509CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom
                };

                var rule1 = typeof(CertificateValidationRuleMock1).AssemblyQualifiedName;
                var rule2 = typeof(CertificateValidationRuleMock).AssemblyQualifiedName;
                var ruleDescriptor = new ValidationRuleDescriptor(rule1);
                var ruleDescriptor2 = new ValidationRuleDescriptor(rule2);
                configuration.ValidationRules.Add(ruleDescriptor);
                configuration.ValidationRules.Add(ruleDescriptor2);

                configuration.ValidationRules.Add(new ValidationRuleDescriptor(rule1));
                var configurationProvider = new CertificateValidationConfigurationProvider(() => configuration);

                var validator = new CertificateValidator(configurationProvider, logger);
                //ACT
                validator.Validate(certificate);
                //ASSERT
                
            }
            finally
            {
                store.Close();
                store.Dispose();
            }
        }

        [Test]
        public void MetadataSerialisationCertificateTest_failed()
        {
            //ARRANGE
            var logger = new LogProviderMock();
            var store = new X509Store("TestCertStore");
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var certificate = store.Certificates.Find(X509FindType.FindBySubjectName, "www.eca-international.com", false)[0];
                var configuration = new CertificateValidationConfiguration
                {
                    X509CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom
                };

                var rule1 = typeof(CertificateValidationRuleMock1).AssemblyQualifiedName;
                var rule2 = typeof(CertificateValidationRuleFailedMock).AssemblyQualifiedName;
                var ruleDescriptor = new ValidationRuleDescriptor(rule1);
                var ruleDescriptor2 = new ValidationRuleDescriptor(rule2);
                configuration.ValidationRules.Add(ruleDescriptor);
                configuration.ValidationRules.Add(ruleDescriptor2);

                configuration.ValidationRules.Add(new ValidationRuleDescriptor(rule1));
                var configurationProvider = new CertificateValidationConfigurationProvider(() => configuration);

                var validator = new CertificateValidator(configurationProvider, logger);
                //ACT

                //ASSERT
                Assert.Throws<InvalidOperationException>(() => validator.Validate(certificate));
            }
            finally
            {
                store.Close();
                store.Dispose();
            }
        }
    }
}