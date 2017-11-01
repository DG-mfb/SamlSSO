using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
            var tt = typeof(CertificateValidatorResolver).AssemblyQualifiedName;
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
            X509Certificate2 rootCert;
            var data = Helper.GetValidBase64EncodedSubjectPublicKeyInfoHashes(StoreName.My, StoreLocation.CurrentUser, X509FindType.FindBySubjectName, "georgiev.danail", out rootCert);
            var r = rootCert.Export(X509ContentType.Pkcs12);
            var chain = new X509Chain
            {
                ChainPolicy = new X509ChainPolicy
                {
                    RevocationFlag = X509RevocationFlag.ExcludeRoot,
                    RevocationMode = X509RevocationMode.NoCheck,
                    VerificationFlags = X509VerificationFlags.AllFlags,
                }
            };

            X509Certificate2 cert;
            var data1 = Helper.GetValidBase64EncodedSubjectPublicKeyInfoHashes(StoreName.My, StoreLocation.CurrentUser, X509FindType.FindBySubjectName, "DG-MFB", out cert);
            chain.ChainPolicy.ExtraStore.Add(rootCert);
            chain.Build(cert);
            data = data.Union(data1);
            
            var validator = new SubjectPublicKeyInfoValidator(data, Security.SubjectPublicKeyInfoAlgorithm.Sha256);
            //ACT
            var result = validator.Validate(this, rootCert, chain, System.Net.Security.SslPolicyErrors.None);
            //ASSERT
            Assert.IsTrue(result);
        }

        [Test]
        public void ReadCertificateAsn1Format()
        {
            byte[] buffer = null;
            using (var fs = new FileStream(@"D:\Dan\Software\Apira\Temp\ApiraEncr.cer", FileMode.Open))
            {
                using (var br = new BinaryReader(fs))
                {
                    buffer = br.ReadBytes((int)fs.Length);
                }
            }
            
            var cert = new X509Certificate2(@"D:\Dan\Software\Apira\Temp\ApiraEncr.cer");
            var foo = Utility.ExtractSpkiBlob(cert);
            var buffer1 = cert.Export(X509ContentType.Pkcs12);

            var pkinfo = Utility.GetSubjectPublicKeyInfo(cert);
            var seq = new List<byte[]>();

            Utility.GetNextSequence(buffer1, seq);
            var asn = new AsnEncodedData(buffer1);
            var asText = asn.Format(true);
            var equal = Enumerable.SequenceEqual(buffer, buffer1);
            Assert.IsTrue(seq.Any(x => x.SequenceEqual(pkinfo)));
            Assert.IsTrue(seq.Any(x => x.SequenceEqual(foo)));
        }
    }
}