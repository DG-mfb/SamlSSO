using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using DeflateCompression;
using Federation.Protocols.Encodiing;
using Federation.Protocols.Test.Mock;
using Kernel.Security.CertificateManagement;
using NUnit.Framework;
using SecurityManagement;
using WsMetadataSerialisation.Serialisation;

namespace Federation.Protocols.Test.Tokens
{
    [TestFixture]
    [Ignore("Infrastruture")]
    internal class MiscellaneousTests
    {
        [Test]
        public async Task GetPlainAsertion_Test()
        {
            //ARRANGE
            var foo = "lVRLj9owEL5X6n%2BIfM87IWARVhRUCamtEKx66GXlOANrKbFT2wH239fOBsKyLdpex5%2F9PWbG04dTXTkHkIoJnqPQC9DD7POnqSJ11eB5q5%2F5Bn63oLRjgFxhe5CjVnIsiGIKc1KDwpri7fz7Nxx5ASZKgdTmOeSslGphxZUmXOcoCsLMDUM3TB%2FDBAchTjIvDbNxmsa%2FkLM0HIwT3el41rpR2PePUIStBzUQby9F23hEV0RR0VDhUVH7zMjxVaP81eaLb6VFgV%2BJPbPcyxwBJU%2BjXVJkUVy6WVyGblJA4o4hHLlBmk3CeFJGZQFPaVyMIoDCDYIxcZNxGblklyQuzUbJhMQBTUiEnJ%2FnmIxN5MzPPheCq7YGuQV5YNT4LeGUI4vQWrKi1fCKYHz%2FDvJVSApdzDnakUqBDW1tImQHuFSG5Jv70TdSaEFFhUwLHadrIu56IC1RTfT967bCSnfXQTFwzfQLml16cTx6JlCXcQ3ytVGksl2Y%2BldEA3ODf5j3Vsu1qBh9ceZVJY4LCUQPxv5flJaEK2akIce%2FMmkCLplVpLriuTxvS4OlsDGzJRm1gP78FvFhl5cLPY9%2Fl6g%2FvpHXx9MvFpRd%2Fw1Gw0k7C1E3RDJl5wxOhGp0Y2nALswuqA3sZnfXkWJqcaZs5%2BooZLk2YwLUED%2FaNBsh9cXGXx4ffPxD8rWnrdlNM%2BeOoTi9LERr9z5442B4RK6WH4397aVrRT2h%2FbP895%2FW7A8%3D";
            var unescaped = Uri.UnescapeDataString(foo);
            var request = "SAMLRequest=lVRLj9owEL5X6n%2BIfM87IWARVhRUCamtEKx66GXlOANrKbFT2wH239fOBsKyLdpex5%2F9PWbG04dTXTkHkIoJnqPQC9DD7POnqSJ11eB5q5%2F5Bn63oLRjgFxhe5CjVnIsiGIKc1KDwpri7fz7Nxx5ASZKgdTmOeSslGphxZUmXOcoCsLMDUM3TB%2FDBAchTjIvDbNxmsa%2FkLM0HIwT3el41rpR2PePUIStBzUQby9F23hEV0RR0VDhUVH7zMjxVaP81eaLb6VFgV%2BJPbPcyxwBJU%2BjXVJkUVy6WVyGblJA4o4hHLlBmk3CeFJGZQFPaVyMIoDCDYIxcZNxGblklyQuzUbJhMQBTUiEnJ%2FnmIxN5MzPPheCq7YGuQV5YNT4LeGUI4vQWrKi1fCKYHz%2FDvJVSApdzDnakUqBDW1tImQHuFSG5Jv70TdSaEFFhUwLHadrIu56IC1RTfT967bCSnfXQTFwzfQLml16cTx6JlCXcQ3ytVGksl2Y%2BldEA3ODf5j3Vsu1qBh9ceZVJY4LCUQPxv5flJaEK2akIce%2FMmkCLplVpLriuTxvS4OlsDGzJRm1gP78FvFhl5cLPY9%2Fl6g%2FvpHXx9MvFpRd%2Fw1Gw0k7C1E3RDJl5wxOhGp0Y2nALswuqA3sZnfXkWJqcaZs5%2BooZLk2YwLUED%2FaNBsh9cXGXx4ffPxD8rWnrdlNM%2BeOoTi9LERr9z5442B4RK6WH4397aVrRT2h%2FbP895%2FW7A8%3D&RelayState=tVFLi8IwEP4rEjx2oyvCYm5S9yGrWKi3IhjTUbOkSclMxSL%2B9013q%2B7jspe9TeZ7hjmxLtUlMMHSGgkKHjtjQJF2FvkzWPBa8Yn%2BWEhfrwdZ1hJT8truok6BynmjN6vogiw2b8HhC7K6zSxiW8jBy8YwkZ7qaR7Cx2Qkxq5ULhAkIvgGj0OJqgCfgj9oBcjE6S91ZxppfZ9lr%2BAtGP50zeNzIDmRJIPMbvWuatePNk%2BctsSnNocj5Jf3N1bU%2BeX342PdgzRV0zK71fz3DiFXNwom%2BmHCCWxlZYgJ8hVEbBOwcKVQpPJWOIkahZUFoCAl0vF8Jga8L1oWipflMrlLFukyuBqnPgME2xOVotdrNmbvkMRwNHoY9tosLrE8BoEHLMMZYHYV2sqY8%2Bp8fgc%3D&SigAlg=http%3A%2F%2Fwww.w3.org%2F2000%2F09%2Fxmldsig%23rsa-sha1&Signature=fSYSDEFSQs2E1lko89N6yhZWd9lptNVuKdI%2BdoKJ0nB%2FI13Mfwv2omvuaa0uaYKjF7hbBHCA1gYg7QTx1UXmyMEhbYk4c0thvvyF3HYlToQeiS9BD1tUTXA305bU0BRIf3kLTuRZzT6Rf5kS9XctAg1NZeBW02ro61YJ01E8Rxxt80GmHcQbEDkc7fIua31xqz%2FYxlz5fwIPmfQgH%2Fei2U7Cabf55GozXcffV7MefhbkyYPm9a2dpGo7Tb1t7THURBrwkUjIbYtcFSbTBVQ52nzGJ%2FoqR8SsJ7bIdgzXjKJdZqVZCxEBpXi69A1xL4llLVUrZmwKKDqplP1jkI9foA%3D%3D";
            var messageEncoding = new MessageEncoding(new DeflateCompressor());
            var decoded = await messageEncoding.DecodeMessage(unescaped);
            var federationMetadataSerialiser = new FederationMetadataSerialiser(new CertificateValidatorMock(), new LogProviderMock());
            var metadataXml = XmlReader.Create(@"D:\Dan\Software\ECA-Interenational\ECA_SPMetadata_20171114.xml");
            var metadata = federationMetadataSerialiser.Deserialise(metadataXml) as EntityDescriptor;
            var spDescriptor = metadata.RoleDescriptors.OfType<ServiceProviderSingleSignOnDescriptor>().First();
            var i = request.IndexOf("Signature");
            var data = request.Substring(0, i - 1);
            var sgn = Uri.UnescapeDataString(request.Substring(i + 10));
            var certificateManager = new CertificateManager(new LogProviderMock());


            //ACT
            var keyDescriptors = spDescriptor.Keys.Where(k => k.Use == KeyType.Signing);
            var validated = false;
            foreach (var k in keyDescriptors.SelectMany(x => x.KeyInfo))
            {
                var binaryClause = k as BinaryKeyIdentifierClause;
                if (binaryClause == null)
                    throw new InvalidOperationException(String.Format("Expected type: {0} but it was: {1}", typeof(BinaryKeyIdentifierClause), k.GetType()));

                var certContent = binaryClause.GetBuffer();
                var cert = new X509Certificate2(certContent);
                validated = this.VerifySignature(request, cert, certificateManager);
                if (validated)
                    break;
            }
            
            //ASSERT
            Assert.True(validated);
        }
        private bool VerifySignature(string request, X509Certificate2 certificate, ICertificateManager certificateManager)
        {
            var i = request.IndexOf("Signature");
            var data = request.Substring(0, i - 1);
            var sgn = Uri.UnescapeDataString(request.Substring(i + 10));

            var validated = certificateManager.VerifySignatureFromBase64(data, sgn, certificate);
            return validated;
        }
    }
}