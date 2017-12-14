using System.Collections.Generic;
using System.Dynamic;
using System.ServiceModel.Security;
using JsonMetadataContextProvider.Test.Mock;
using Kernel.Federation.FederationPartner;
using NUnit.Framework;
using Serialisation.JSON;
using Serialisation.JSON.SettingsProviders;

namespace JsonMetadataContextProvider.Test
{
    [TestFixture]
    public class FederationPartyContextBuilderTest
    {
        [Test]
        public void JsonFederationConfigurationTest()
        {
            //ARRANGE
            var configurations = new List<FederationPartyConfiguration>();
            var inlineProvider = new InlineMetadataContextProvider.FederationPartyContextBuilder();
            var jsonSerialiser = new NSJsonSerializer(new DefaultSettingsProvider());
            var config1 = inlineProvider.BuildContext("atlasCopco",  @"file://D:\Dan\Software\ECA-Interenational\Metadata\atlasCopco\federation_metadata.xml");
            configurations.Add(config1);
            var config2 = inlineProvider.BuildContext("local", "https://dg-mfb/idp/shibboleth");
            configurations.Add(config2);
            var config3 = inlineProvider.BuildContext("testshib", "https://www.testshib.org/metadata/testshib-providers.xml");
            configurations.Add(config3);
            var serialised = jsonSerialiser.Serialize(configurations);
            var cache = new MockCacheProvider();
            var jsonProvider = new JsonMetadataContextProvider.FederationPartyContextBuilder(jsonSerialiser, cache, t => serialised);
            //ACT
            var found1 = jsonProvider.BuildContext("atlasCopco");
            var found2 = jsonProvider.BuildContext("local");
            //ASSERT
            Assert.IsNotNull(found1);
            Assert.AreEqual("atlasCopco", found1.FederationPartyId);
            Assert.AreEqual("local", found2.FederationPartyId);
        }

        [Test]
        public void JsonBackChannelConfigurationTest()
        {
            //ARRANGE
            var configurations = new List<object>();
            var inlineProvider = new InlineMetadataContextProvider.Security.CertificateValidationConfigurationProvider();
            var jsonSerialiser = new NSJsonSerializer(new DefaultSettingsProvider());
            var config1 = inlineProvider.GetConfiguration("atlasCopco");
            dynamic expando1 = new ExpandoObject();
            expando1.Id = "atlasCopco";
            expando1.Configuration = config1;
            configurations.Add(expando1);

            var config2 = inlineProvider.GetConfiguration("testshib");
            dynamic expando2 = new ExpandoObject();
            expando2.Id = "testshib";
            expando2.Configuration = config2;
            configurations.Add(expando2);
            

            var config3 = inlineProvider.GeBackchannelConfiguration("atlasCopco");
            
            dynamic expando3 = new ExpandoObject();
            expando3.Id = "atlasCopco";
            expando3.Metadata = "";
            expando3.Configuration = config3;
            configurations.Add(expando3);

            var config4 = inlineProvider.GeBackchannelConfiguration("testshib");
            dynamic expando4 = new ExpandoObject();
            expando4.Id = "testshib";
            expando4.MetadataAddress = "https://www.testshib.org/metadata/testshib-providers.xml";
            expando4.Configuration = config4;
            configurations.Add(expando4);
            

            var serialised = jsonSerialiser.Serialize(configurations);
            
            var cache = new MockCacheProvider();
            var jsonProvider = new JsonMetadataContextProvider.Security.CertificateValidationConfigurationProvider(jsonSerialiser, cache, t => serialised);
            //ACT
            
            var found1 = jsonProvider.GeBackchannelConfiguration("atlasCopco");
            var found2 = jsonProvider.GeBackchannelConfiguration("testshib");
            //ASSERT
            Assert.IsNotNull(found1);
            Assert.False(found1.UsePinningValidation);
            Assert.False(found2.UsePinningValidation);
        }

        [Test]
        public void JsonCertificateValidationConfigurationTest()
        {
            //ARRANGE
            var configurations = new List<object>();
            var inlineProvider = new InlineMetadataContextProvider.Security.CertificateValidationConfigurationProvider();
            var jsonSerialiser = new NSJsonSerializer(new DefaultSettingsProvider());
            var config1 = inlineProvider.GetConfiguration("atlasCopco");
            dynamic expando1 = new ExpandoObject();
            expando1.Id = "atlasCopco";
            expando1.Configuration = config1;
            configurations.Add(expando1);

            var config2 = inlineProvider.GetConfiguration("testshib");
            dynamic expando2 = new ExpandoObject();
            expando2.Id = "testshib";
            expando2.Configuration = config2;
            configurations.Add(expando2);


            var config3 = inlineProvider.GeBackchannelConfiguration("atlasCopco");

            dynamic expando3 = new ExpandoObject();
            expando3.Id = "atlasCopco";
            expando3.Metadata = "";
            expando3.Configuration = config3;
            configurations.Add(expando3);

            var config4 = inlineProvider.GeBackchannelConfiguration("testshib");
            dynamic expando4 = new ExpandoObject();
            expando4.Id = "testshib";
            expando4.MetadataAddress = "https://www.testshib.org/metadata/testshib-providers.xml";
            expando4.Configuration = config4;
            configurations.Add(expando4);


            var serialised = jsonSerialiser.Serialize(configurations);

            var cache = new MockCacheProvider();
            var jsonProvider = new JsonMetadataContextProvider.Security.CertificateValidationConfigurationProvider(jsonSerialiser, cache, t => serialised);
            //ACT

            var found1 = jsonProvider.GetConfiguration("atlasCopco");
            var found2 = jsonProvider.GetConfiguration("testshib");
            //ASSERT
            Assert.IsNotNull(found1);
            Assert.AreEqual(X509CertificateValidationMode.Custom, found1.X509CertificateValidationMode);
            Assert.AreEqual(X509CertificateValidationMode.Custom, found2.X509CertificateValidationMode);
        }
    }
}