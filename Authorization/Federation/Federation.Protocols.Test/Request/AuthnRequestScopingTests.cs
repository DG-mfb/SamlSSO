﻿using System;
using System.Collections.Generic;
using Federation.Protocols.Request;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.Constants;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.Protocols.Request;
using NUnit.Framework;
using Shared.Federtion.Models;

namespace Federation.Protocols.Test.Request
{
    [TestFixture]
    internal class AuthnRequestScopingTests
    {
        [Test]
        public void BuildAuthnRequest_test_scoping_default()
        {
            //ARRANGE
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local");
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContex, supportedNameIdentifierFormats);
            var requestConfiguration = federationContex.GetAuthnRequestConfigurationFromContext(Guid.NewGuid().ToString());
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            
            //ACT
            var authnRequest = RequestHelper.BuildRequest(authnRequestContext) as AuthnRequest;

            //ASSERT
            Assert.NotNull(authnRequest);
            Assert.IsNotNull(authnRequest.Scoping);
            Assert.AreEqual("0", authnRequest.Scoping.ProxyCount);
            Assert.AreEqual(1, authnRequest.Scoping.RequesterId.Length);
            Assert.AreEqual(requestConfiguration.EntityId, authnRequest.Scoping.RequesterId[0]);
        }

        [Test]
        public void BuildAuthnRequest_test_scoping_default_overwritten()
        {
            //ARRANGE
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var scopingConfiguration = new ScopingConfiguration("http://localhost:59611/") { PoxyCount = 10 };
            var federationContext = federationPartyContextBuilder.BuildContext("local", scopingConfiguration);
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContext, supportedNameIdentifierFormats);
            var requestConfiguration = federationContext.GetAuthnRequestConfigurationFromContext(Guid.NewGuid().ToString());
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            
            //ACT
            var authnRequest = RequestHelper.BuildRequest(authnRequestContext) as AuthnRequest;

            //ASSERT
            Assert.NotNull(authnRequest);
            Assert.IsNotNull(authnRequest.Scoping);
            Assert.AreEqual("10", authnRequest.Scoping.ProxyCount);
            Assert.AreEqual(1, authnRequest.Scoping.RequesterId.Length);
            Assert.AreEqual("http://localhost:59611/", authnRequest.Scoping.RequesterId[0]);
        }

        [Test]
        public void BuildAuthnRequest_test_scoping_default_overwritten_2_requesters()
        {
            //ARRANGE
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var scopingConfiguration = new ScopingConfiguration("http://localhost:59611/", "http://localhost:59612/") { PoxyCount = 10 };
            var federationContext = federationPartyContextBuilder.BuildContext("local", scopingConfiguration);
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContext, supportedNameIdentifierFormats);
            var requestConfiguration = federationContext.GetAuthnRequestConfigurationFromContext(Guid.NewGuid().ToString());
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();

            //ACT
            var authnRequest = RequestHelper.BuildRequest(authnRequestContext) as AuthnRequest;

            //ASSERT
            Assert.NotNull(authnRequest);
            Assert.IsNotNull(authnRequest.Scoping);
            Assert.AreEqual("10", authnRequest.Scoping.ProxyCount);
            Assert.AreEqual(2, authnRequest.Scoping.RequesterId.Length);
            Assert.AreEqual("http://localhost:59611/", authnRequest.Scoping.RequesterId[0]);
            Assert.AreEqual("http://localhost:59612/", authnRequest.Scoping.RequesterId[1]);
        }
    }
}