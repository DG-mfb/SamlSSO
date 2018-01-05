using System;
using System.Collections.Generic;
using DeflateCompression;
using Federation.Protocols.Encodiing;
using Federation.Protocols.Request;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Request;
using NUnit.Framework;
using Serialisation.Xml;
using Shared.Federtion.Constants;
using Shared.Federtion.Models;

namespace Federation.Protocols.Test.Request
{
    [TestFixture]
    internal class RequestTypeResolverTests
    {
        [Test]
        public  void AuthnRequestType_test()
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
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            var authnRequest = RequestHelper.BuildRequest(authnRequestContext);
            var typeResolver = new RequestTypeResolver();
            //ACT
            var serialised = serialiser.Serialize(authnRequest);
            var type = typeResolver.ResolveMessageType(serialised);

            //ASSERT
            
            Assert.AreEqual(typeof(AuthnRequest), type);
        }

        [Test]
        public  void LogoutRequestType_test_test()
        {
            //ARRANGE
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local");
            //var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new LogoutRequestContext(requestUri, new Uri("http://localhost"), federationContex);

            var xmlSerialiser = new XMLSerialiser();
            var compressor = new DeflateCompressor();
            var encoder = new MessageEncoding(compressor);
            var logger = new LogProviderMock();
            var serialiser = new RequestSerialiser(xmlSerialiser, encoder, logger) as IRequestSerialiser;
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            var authnRequest = RequestHelper.BuildRequest(authnRequestContext);
            var typeResolver = new RequestTypeResolver();
            //ACT
            var serialised = serialiser.Serialize(authnRequest);
            var type = typeResolver.ResolveMessageType(serialised);
            //ASSERT

            Assert.AreEqual(typeof(LogoutRequest), type);
        }
    }
}