using System.Collections.Generic;
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
        public void CrateJsonMetadata()
        {
            //ARRANGE
            var configurations = new List<FederationPartyConfiguration>();
            var inlineProvider = new InlineMetadataContextProvider.FederationPartyContextBuilder();
            var jsonSerialiser = new NSJsonSerializer(new DefaultSettingsProvider());
            var config1 = inlineProvider.BuildContext("atlasCopco",  @"file://D:\Dan\Software\ECA-Interenational\Metadata\atlasCopco\federation_metadata.xml");
            configurations.Add(config1);
            var config2 = inlineProvider.BuildContext("local", "https://dg-mfb/idp/shibboleth");
            configurations.Add(config2);
            var config3 = inlineProvider.BuildContext("testShib", "https://www.testshib.org/metadata/testshib-providers.xml");
            configurations.Add(config3);
            var serialised = jsonSerialiser.Serialize(configurations);
            var cache = new MockCacheProvider();
            var jsonProvider = new JsonMetadataContextProvider.FederationPartyContextBuilder(jsonSerialiser, cache, () => serialised);
            //ACT
            var found1 = jsonProvider.BuildContext("atlasCopco");
            var found2 = jsonProvider.BuildContext("local");
            //ASSERT
            Assert.IsNotNull(found1);
            Assert.AreEqual("atlasCopco", found1.FederationPartyId);
            Assert.AreEqual("local", found2.FederationPartyId);
        }
    }
}