using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeflateCompression;
using Federation.Protocols.Bindings.HttpPost;
using Federation.Protocols.Encodiing;
using Federation.Protocols.RelayState;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.Protocols;
using NUnit.Framework;
using Serialisation.JSON;
using Serialisation.JSON.SettingsProviders;
using Shared.Federtion.Constants;

namespace Federation.Protocols.Test.Encoding
{
    [TestFixture]
    internal class PostBindingDecodingTest
    {
        [Test]
        public async Task DecodeTest()
        {
            //ARRANGE
            var inResponseTo = Guid.NewGuid().ToString();

            var response = ResponseFactoryMock.GetTokenResponseSuccess(inResponseTo, StatusCodes.Success);
            var logger = new LogProviderMock();
            var serialised = ResponseFactoryMock.Serialize(response);
            var responseToBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(serialised));
            var relayState = new Dictionary<string, object> { { "relayState", "Test state" } };
            var compressor = new DeflateCompressor();
            var messageEncoder = new MessageEncoding(compressor);
            var jsonSerialiser = new NSJsonSerializer(new DefaultSettingsProvider());
            var serialiser = new RelaystateSerialiser(jsonSerialiser, messageEncoder, logger) as IRelayStateSerialiser;
            var serialisedRelaySatate = await serialiser.Serialize(relayState);
            var relayStateHandler = new RelayStateHandler(serialiser, logger);
            var form = new Dictionary<string, string>
            {
                { HttpRedirectBindingConstants.SamlResponse, responseToBase64 },
                { HttpRedirectBindingConstants.RelayState, serialisedRelaySatate }
            };

            var decoder = new PostBindingDecoder(logger, relayStateHandler);
            //ACT
            var result = await decoder.Decode(form);
            var stateFromResult = result[HttpRedirectBindingConstants.RelayState] as IDictionary<string, object>;
            //ASSERT
            Assert.IsNotNull(stateFromResult);
            Assert.AreEqual(serialised, result[HttpRedirectBindingConstants.SamlResponse]);
            Assert.IsTrue(relayState.SequenceEqual(stateFromResult));
        }
    }
}
