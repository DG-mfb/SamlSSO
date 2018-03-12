using System;
using System.Collections.Generic;
using Federation.Protocols.Request;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols.Request;
using NUnit.Framework;
using Shared.Federtion.Models;

namespace Federation.Protocols.Test.Request
{
    [TestFixture]
    internal class AuthnRequestAssertionIndexPointTests
    {
        [Test]
        public void BuildAuthnRequest_test_default_intex_endpoint()
        {
            //ARRANGE
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local", 0);
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContex, supportedNameIdentifierFormats);
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            //ACT
            var config = federationContex.GetAuthnRequestConfigurationFromContext(Guid.NewGuid().ToString());
            var authnRequest = RequestHelper.BuildRequest(authnRequestContext) as AuthnRequest;
            //ASSERT
            Assert.IsNotNull(config.RequestedAuthnContextConfiguration);
            Assert.AreEqual(0, authnRequest.AssertionConsumerServiceIndex);
        }

        [Test]
        public void BuildAuthnRequest_test_default_overwritten_intex_endpoint()
        {
            //ARRANGE
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local", 1);
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContex, supportedNameIdentifierFormats);
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            //ACT
            var config = federationContex.GetAuthnRequestConfigurationFromContext(Guid.NewGuid().ToString());
            var authnRequest = RequestHelper.BuildRequest(authnRequestContext) as AuthnRequest;
            //ASSERT
            Assert.IsNotNull(config.RequestedAuthnContextConfiguration);
            Assert.AreEqual(1, authnRequest.AssertionConsumerServiceIndex);
        }
    }
}