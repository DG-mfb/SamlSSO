using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeflateCompression;
using Federation.Protocols.Encodiing;
using Federation.Protocols.Request;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Request;
using Kernel.Serialisation;
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
        public async Task AuthnRequestSerialiser_with_compression_test()
        {
            //ARRANGE
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local");
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContex, supportedNameIdentifierFormats);

            var xmlSerialiser = new XMLSerialiser();
            var compressor = new DeflateCompressor();
            var encoder = new MessageEncoding(compressor);
            var logger = new LogProviderMock();
            var serialiser = new RequestSerialiser(xmlSerialiser, encoder, logger) as IRequestSerialiser;
            RequestHelper.GetBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            var authnRequest = RequestHelper.BuildRequest(authnRequestContext);

            //ACT
            var serialised = await serialiser.SerializeAndCompress(authnRequest);
            var deserialised = await serialiser.DecompressAndDeserialize<AuthnRequest>(serialised);
            //ASSERT
            Assert.NotNull(serialised);
            Assert.AreEqual(authnRequest.Issuer.Value, deserialised.Issuer.Value);
        }

        [Test]
        public  void AuthnRequestSerialiser_test()
        {
            //ARRANGE
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local");
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContex, supportedNameIdentifierFormats);

            var xmlSerialiser = new XMLSerialiser();
            var compressor = new DeflateCompressor();
            var encoder = new MessageEncoding(compressor);
            var logger = new LogProviderMock();
            var serialiser = new RequestSerialiser(xmlSerialiser, encoder, logger) as ISerializer;
            RequestHelper.GetBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            var authnRequest = RequestHelper.BuildRequest(authnRequestContext);

            //ACT
            var serialised = serialiser.Serialize(authnRequest);
            var deserialised = serialiser.Deserialize<AuthnRequest>(serialised);
            //ASSERT
            Assert.NotNull(serialised);
            Assert.AreEqual(authnRequest.Issuer.Value, deserialised.Issuer.Value);
        }
    }
}