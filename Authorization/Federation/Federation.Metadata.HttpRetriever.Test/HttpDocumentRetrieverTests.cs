using System;
using System.Threading;
using System.Threading.Tasks;
using Federation.Metadata.HttpRetriever.Test.Mock;
using NUnit.Framework;

namespace Federation.Metadata.HttpRetriever.Test
{
    [TestFixture]
    public class HttpDocumentRetrieverTests
    {
        [Test]
        public async Task SecureChanelTestShibMetadata_load()
        {
            //ARRANGE
            var certValidator = new CertificateValidatorMock();
            
            var documentRetriever = new HttpDocumentRetrieverMock(certValidator);
            var address = "https://localhost";
            //ACT
            var doc = await documentRetriever.GetDocumentAsync(address, CancellationToken.None);
            //ASSERT
            Assert.False(String.IsNullOrWhiteSpace(doc));
            Assert.AreEqual("Content", doc);
        }
    }
}