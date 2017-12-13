using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineMetadataContextProvider;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.MetaData;
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
            var config2 = inlineProvider.BuildContext("atlasCopco", @"file://D:\Dan\Software\ECA-Interenational\Metadata\atlasCopco\federation_metadata.xml");
            configurations.Add(config2);
            var serialised = jsonSerialiser.Serialize(configurations);
            var jsonProvider = new JsonMetadataContextProvider.FederationPartyContextBuilder(jsonSerialiser, null, () => serialised);
            //ACT
            var found = jsonProvider.BuildContext("atlasCopco");
            //ASSERT
            Assert.IsNotNull(found);
            Assert.AreEqual("atlasCopco", found.FederationPartyId);
        }
    }
}