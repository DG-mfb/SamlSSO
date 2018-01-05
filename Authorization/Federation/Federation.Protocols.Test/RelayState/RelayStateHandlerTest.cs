using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeflateCompression;
using Federation.Protocols.Encodiing;
using Federation.Protocols.RelayState;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Request;
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
            var handler = new RelayStateHandler(serialiser, logger);
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
            var logger = new LogProviderMock();
            var handler = new RelayStateAppender(logger);
            //ACT
            var federationPartyContextBuilderMock = new FederationPartyContextBuilderMock();
            var configuration = federationPartyContextBuilderMock.BuildContext("local");
            var authnRequestContext = new AuthnRequestContext(new Uri("http://localhost"), new Uri("http://localhost"), configuration, new []{new Uri("http://localhost") });
            await handler.BuildRelayState(authnRequestContext);
            //ASSERT
            Assert.AreEqual(3, authnRequestContext.RelyingState.Count);
            Assert.AreEqual("local", authnRequestContext.RelyingState["federationPartyId"]);
            Assert.AreEqual(authnRequestContext.RequestId, authnRequestContext.RelyingState.ElementAt(1).Value);
            Assert.AreEqual("http://localhost/", authnRequestContext.RelyingState.ElementAt(2).Value.ToString());
        }
    }
}