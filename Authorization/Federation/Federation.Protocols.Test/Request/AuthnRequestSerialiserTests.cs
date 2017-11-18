using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeflateCompression;
using Federation.Protocols.Endocing;
using Federation.Protocols.Request;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.Protocols;
using NUnit.Framework;
using Serialisation.Xml;
using Shared.Federtion.Constants;
using Shared.Federtion.Models;

namespace Federation.Protocols.Test.Request
{
    [TestFixture]
    internal class AuthnRequestSerialiserTests
    {
        [Test]
        public async Task AuthnRequestSerialiser_test()
        {
            //ARRANGE
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local");
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new AuthnRequestContext(requestUri, federationContex, supportedNameIdentifierFormats);

            var xmlSerialiser = new XMLSerialiser();
            var compressor = new DeflateCompressor();
            var encoder = new MessageEncoding(compressor);
            var logger = new LogProviderMock();
            var serialiser = new AuthnRequestSerialiser(xmlSerialiser, encoder, logger) as IAuthnRequestSerialiser;
            AuthnRequestHelper.GetBuilders = AuthnRequestBuildersFactoryMock.GetBuildersFactory();
            var authnRequest = AuthnRequestHelper.BuildAuthnRequest(authnRequestContext);

            //ACT
            var serialised = await serialiser.SerializeAndCompress(authnRequest);
            var deserialised = await serialiser.DecompressAndDeserialize<AuthnRequest>(serialised);
            //ASSERT
            Assert.NotNull(serialised);
            Assert.AreEqual(authnRequest.Issuer.Value, deserialised.Issuer.Value);
        }
    }
}
