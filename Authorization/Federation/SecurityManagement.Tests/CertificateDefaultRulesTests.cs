using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Kernel.Security.Validation;
using NUnit.Framework;
using SecurityManagement.CertificateValidationRules;
using SecurityManagement.Tests.Mock;

namespace SecurityManagement.Tests
{
    [TestFixture]
    internal class CertificateDefaultRulesTests
    {
        [Test]
        public async Task EffectiveDateRuleTest()
        {
            //ARRANGE
            var store = new X509Store("TestCertStore");
            try
            {
                var logger = new LogProviderMock();
                store.Open(OpenFlags.ReadOnly);
                var certificate = store.Certificates.Find(X509FindType.FindBySubjectName, "www.eca-international.com", false)[0];
                var context = new CertificateValidationContext(certificate);
                var rule = new EffectiveDateRule(logger);
                //ACT
                await rule.Validate(context, c => Task.CompletedTask);
                //ASSERT

            }
            finally
            {
                store.Close();
                store.Dispose();
            }
        }

        [Test]
        public async Task ExpirationDateRuleTest()
        {
            //ARRANGE
            var store = new X509Store("TestCertStore");
            try
            {
                var logger = new LogProviderMock();
                store.Open(OpenFlags.ReadOnly);
                var certificate = store.Certificates.Find(X509FindType.FindBySubjectName, "www.eca-international.com", false)[0];
                var context = new CertificateValidationContext(certificate);
                var rule = new ExpirationDateRule(logger);
                //ACT
                await rule.Validate(context, c => Task.CompletedTask);
                //ASSERT

            }
            finally
            {
                store.Close();
                store.Dispose();
            }
        }
    }
}
