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
            
            var documentRetriever = new HttpDocumentRetriever(certValidator);
            var address = "https://www.testshib.org/metadata/testshib-providers.xml";
            //ACT
            var doc = await documentRetriever.GetDocumentAsync(address, CancellationToken.None);
            //ASSERT
            Assert.False(String.IsNullOrWhiteSpace(doc));

        }
    }
}