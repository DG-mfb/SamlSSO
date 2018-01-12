using System;
using System.IdentityModel.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Federation.Metadata.Consumer.Tests.Mock;
using Federation.Metadata.FederationPartner.Configuration;
using Kernel.Federation.FederationPartner;
using NUnit.Framework;
using SecurityManagement;
using WsMetadataSerialisation.Serialisation;

namespace Federation.Metadata.Consumer.Tests
{
    [TestFixture]
    public class MetadataRetrieverMetadataTests
    {
        [Test]
        public async Task HttpDocumentRetrieverTest()
        {
            //ARRANGE
            var certValidator = new CertificateValidatorMock();
            var documentRetrieer = new HttpDocumentRetrieverMock(certValidator);

            //ACT
            var document = await documentRetrieer.GetDocumentAsync("https://localhost", new CancellationToken());
            
            //ASSERT
            Assert.IsFalse(String.IsNullOrWhiteSpace(document));
            Assert.AreEqual(HttpClientMock.Metadata, document);
        }

        [Test]
        public async Task WsFederationConfigurationRetrieverTest()
        {
            //ARRANGE
            var logger = new LogProviderMock();
            var bckChannelcertValidator = new CertificateValidatorMock();
           
            var documentRetrieer = new HttpDocumentRetrieverMock(bckChannelcertValidator);
            var configurationProvider = new CertificateValidationConfigurationProvider();
            var certValidator = new CertificateValidator(configurationProvider, logger);
            
            var serialiser = new FederationMetadataSerialiser(certValidator, logger);
            var configurationRetriever = new WsFederationConfigurationRetriever(_ => documentRetrieer, serialiser);
            
            //ACT
            var context = new FederationPartyConfiguration("local", "https://localhost");
            var baseMetadata = await configurationRetriever.GetAsync(context, new CancellationToken());
            var metadata = baseMetadata as EntityDescriptor;
            //ASSERT
            Assert.IsTrue(metadata != null);
            Assert.AreEqual(1, metadata.RoleDescriptors.Count);
        }
    }
}