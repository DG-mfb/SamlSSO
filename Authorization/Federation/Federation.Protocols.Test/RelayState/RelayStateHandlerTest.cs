using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeflateCompression;
using Federation.Protocols.Endocing;
using Federation.Protocols.RelayState;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.Protocols;
using NUnit.Framework;
using Serialisation.JSON;
using Serialisation.JSON.SettingsProviders;

namespace Federation.Protocols.Test.RelayState
{
    [TestFixture]
    internal class RelayStateHandlerTest
    {
        [Test]
        public async Task GetRelayStateFromFormDataTest()
        {
            //ARRANGE
            var relayState = new Dictionary<string, object> { { "relayState", "Test state" } };
            var form = new Dictionary<string, string>();
            var compressor = new DeflateCompressor();
            var messageEncoder = new MessageEncoding(compressor);
            var jsonSerialiser = new NSJsonSerializer(new DefaultSettingsProvider());
            var logger = new LogProviderMock();
            var serialiser = new RelaystateSerialiser(jsonSerialiser, messageEncoder, logger) as IRelayStateSerialiser;
            var handler = new RelayStateHandler(serialiser);
            //ACT
            var serialised = await serialiser.Serialize(relayState);
            form.Add("RelayState", serialised);
            var deserialised = await handler.GetRelayStateFromFormData(form) as Dictionary<string, object>;
            //ASSERT
            Assert.AreEqual(relayState.Count, deserialised.Count);
            Assert.AreEqual(relayState["relayState"], deserialised["relayState"]);
        }

        [Test]
        public async Task BuildRelayStateTest()
        {
            //ARRANGE
            var serialiser = new RelayStateSerialiserMock() as IRelayStateSerialiser;
            var handler = new RelayStateHandler(serialiser);
            //ACT
            var federationPartyContextBuilderMock = new FederationPartyContextBuilderMock();
            var configuration = federationPartyContextBuilderMock.BuildContext("local");
            var authnRequestContext = new AuthnRequestContext(new Uri("http://localhost"),configuration, new []{new Uri("http://localhost") });
            await handler.BuildRelayState(authnRequestContext);
            //ASSERT
            Assert.AreEqual(2, authnRequestContext.RelyingState.Count);
            Assert.AreEqual("local", authnRequestContext.RelyingState["federationPartyId"]);
            Assert.AreEqual("assertionConsumerServices", authnRequestContext.RelyingState.ElementAt(1).Key);
        }
    }
}