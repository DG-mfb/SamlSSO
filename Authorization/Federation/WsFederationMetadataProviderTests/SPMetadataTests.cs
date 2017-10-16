using System;
using System.IdentityModel.Metadata;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using InlineMetadataContextProvider;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.MetaData;
using NUnit.Framework;
using SecurityManagement;
using SecurityManagement.CertificateValidationRules;
using WsFederationMetadataProvider.Metadata;
using WsFederationMetadataProviderTests.Mock;
using WsMetadataSerialisation.Serialisation;

namespace WsFederationMetadataProviderTests
{
    [TestFixture]
    public class SPMetadataTests
    {
        [Test]
        public async Task SPMetadataGenerationTest()
        {
            ////ARRANGE
           
            var result = String.Empty;
            var metadataWriter = new TestMetadatWriter(el => result = el.OuterXml);

            var logger = new LogProviderMock();
            var contextBuilder = new InlineMetadataContextBuilder();
            var metadataRequest = new MetadataGenerateRequest(MetadataType.SP, "local");
            var metadataContext = contextBuilder.BuildContext(metadataRequest);
            var context = new FederationPartyConfiguration(metadataRequest.FederationPartyId, "localhost");
            context.MetadataContext = metadataContext;
            var configurationProvider = new CertificateValidationConfigurationProvider();
            var certificateValidator = new CertificateValidator(configurationProvider, logger);
            var ssoCryptoProvider = new CertificateManager(logger);
            
            var metadataSerialiser = new FederationMetadataSerialiser(certificateValidator, logger);
            var metadataDispatcher = new FederationMetadataDispatcherMock(() => new[] { metadataWriter });
            
            var sPSSOMetadataProvider = new SPSSOMetadataProvider(metadataDispatcher, ssoCryptoProvider, metadataSerialiser, g => context, logger);
            
            //ACT
            await sPSSOMetadataProvider.CreateMetadata(metadataRequest);
            //ASSERT
            Assert.IsFalse(String.IsNullOrWhiteSpace(result));
        }
        
        [Test]
        public async Task SPMetadata_serialise_deserialise_Test()
        {
            ////ARRANGE
            var logger = new LogProviderMock();
            string metadataXml = String.Empty;
            var metadataWriter = new TestMetadatWriter(el => metadataXml = el.OuterXml);
            CertificateValidationRulesFactory.InstanceCreator = ValidationRuleInstanceCreatorMock.CreateInstance;
            var contextBuilder = new InlineMetadataContextBuilder();
            var metadataRequest = new MetadataGenerateRequest(MetadataType.SP, "local");
            var metadataContext = contextBuilder.BuildContext(metadataRequest);
            var context = new FederationPartyConfiguration(metadataRequest.FederationPartyId, "localhost");
            context.MetadataContext = metadataContext;
            
            var configurationProvider = new CertificateValidationConfigurationProvider();
            var certificateValidator = new CertificateValidator(configurationProvider, logger);
            var ssoCryptoProvider = new CertificateManager(logger);
            
            var metadataSerialiser = new FederationMetadataSerialiser(certificateValidator, logger);

            var metadataDispatcher = new FederationMetadataDispatcherMock(() => new[] { metadataWriter });
            
            var sPSSOMetadataProvider = new SPSSOMetadataProvider(metadataDispatcher, ssoCryptoProvider, metadataSerialiser, g => context, logger);
            
            //ACT
            await sPSSOMetadataProvider.CreateMetadata(metadataRequest);
            var xmlReader = XmlReader.Create(new StringReader(metadataXml));
            var deserialisedMetadata = metadataSerialiser.Deserialise(xmlReader) as EntityDescriptor;

            //ASSERT
            Assert.IsFalse(String.IsNullOrWhiteSpace(metadataXml));
            Assert.AreEqual(1, deserialisedMetadata.RoleDescriptors.Count);
        }
    }
}