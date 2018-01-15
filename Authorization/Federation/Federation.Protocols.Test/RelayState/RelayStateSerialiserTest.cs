﻿using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DeflateCompression;
using Federation.Protocols.Encodiing;
using Federation.Protocols.RelayState;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.Protocols;
using NUnit.Framework;
using Serialisation.JSON;
using Serialisation.JSON.SettingsProviders;

namespace Federation.Protocols.Test.RelayState
{
    [TestFixture]
    internal class RelayStateSerialiserTest
    {
        [Test]
        public async Task SerialiseDeserialiseTest()
        {
            //ARRANGE
            var relayState = new Dictionary<string, object> { { "relayState", "Test state" } };
            var compressor = new DeflateCompressor();
            var messageEncoder = new MessageEncoding(compressor);
            var jsonSerialiser = new NSJsonSerializer(new DefaultSettingsProvider());
            var logger = new LogProviderMock();
            var serialiser = new RelaystateSerialiser(jsonSerialiser, messageEncoder, logger) as IRelayStateSerialiser;
            //ACT
            var serialised = await serialiser.Serialize(relayState);
            var deserialised = await serialiser.Deserialize(serialised) as Dictionary<string, object>;

            //ASSERT
            Assert.AreEqual(relayState.Count, deserialised.Count);
            Assert.AreEqual(relayState["relayState"], deserialised["relayState"]);
        }

        [Test]
        public async Task SerialiseDeserialise_with_data_protectionTest()
        {
            //ARRANGE
            var relayState = new Dictionary<string, object> { { "relayState", "Test state" } };
            var compressor = new DeflateCompressor();
            var messageEncoder = new MessageEncoding(compressor);
            var jsonSerialiser = new NSJsonSerializer(new DefaultSettingsProvider());
            var logger = new LogProviderMock();
            var relayStateSerialiser = new RelaystateSerialiser(jsonSerialiser, messageEncoder, logger);
            relayStateSerialiser.DataProtector = new Kernel.Cryptography.DataProtection.DpapiDataProtector("SSO", "saml", "relaystate");
            var serialiser = relayStateSerialiser as IRelayStateSerialiser;
            
            //ACT
            var serialised = await serialiser.Serialize(relayState);
            var deserialised = await serialiser.Deserialize(serialised) as Dictionary<string, object>;

            //ASSERT
            Assert.AreEqual(relayState.Count, deserialised.Count);
            Assert.AreEqual(relayState["relayState"], deserialised["relayState"]);
        }
    }
}