using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;

namespace Microsoft.Owin.CertificateValidators.Tests
{
    [TestFixture]
    [Ignore("Infrastructure tests")]
    internal class PinningValidatorsTests
    {
        [Test]
        public void GetCertificateSubjectPublicKeyInfo()
        {
            //ARRANGE
            using (var store = new X509Store("TestCertStore", StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.OpenExistingOnly);
                var cert = store.Certificates[0];
                //ACT
                var pkinfo = Utility.GetSubjectPublicKeyInfo(cert);
                var base64Pkif = Utility.HashSpki(pkinfo);
                //ASSERT
                Assert.IsNotNull(base64Pkif);
            }
        }

        [Test]
        public void CertificateSubjectPublicKeyInfoValidatorTest()
        {
            //ARRANGE
            X509Certificate2 cert;
            var data = Helper.GetValidBase64EncodedSubjectPublicKeyInfoHashes(StoreName.Root, StoreLocation.LocalMachine, X509FindType.FindBySubjectName, "Certum CA", out cert);
            var validator = new SubjectPublicKeyInfoValidator(data, Security.SubjectPublicKeyInfoAlgorithm.Sha256);
            var chain = new X509Chain(true) { ChainPolicy = new X509ChainPolicy() };
            chain.Build(cert);
            //ACT
            var result = validator.Validate(this, cert, chain, System.Net.Security.SslPolicyErrors.None);
            //ASSERT
            Assert.IsTrue(result);
        }
    }
}