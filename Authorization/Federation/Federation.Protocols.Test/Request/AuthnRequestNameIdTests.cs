﻿using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Protocols.Request;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols.Request;
using NUnit.Framework;
using Shared.Federtion.Models;

namespace Federation.Protocols.Test.Request
{
    [TestFixture]
    public class AuthnRequestNameIdTests
    {
        [Test]
        public void BuildAuthnRequest_test_nameid_format_match()
        {
            //ARRANGE
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local", NameIdentifierFormats.Transient);
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContex, supportedNameIdentifierFormats);
            var requestConfiguration = federationContex.GetAuthnRequestConfigurationFromContext(Guid.NewGuid().ToString());
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            //ACT
            var authnRequest = RequestHelper.BuildRequest(authnRequestContext) as AuthnRequest;
            var audience = ((AudienceRestriction)authnRequest.Conditions.Items.Single())
                .Audience
                .Single();

            //ASSERT
            Assert.NotNull(authnRequest);
            Assert.AreEqual(requestConfiguration.IsPassive, authnRequest.IsPassive);
            Assert.AreEqual(requestConfiguration.ForceAuthn, authnRequest.ForceAuthn);
            Assert.AreEqual("2.0", authnRequest.Version);
            //issuer
            Assert.AreEqual(requestConfiguration.EntityId, authnRequest.Issuer.Value);
            Assert.AreEqual(NameIdentifierFormats.Entity, authnRequest.Issuer.Format);
            //audience
            Assert.AreEqual(requestConfiguration.AudienceRestriction.Count, authnRequest.Conditions.Items.Count);
            Assert.AreEqual(requestConfiguration.AudienceRestriction.Single(), audience);
            //nameIdPolicy
            Assert.IsFalse(authnRequest.NameIdPolicy.AllowCreate);
            Assert.AreEqual(authnRequest.NameIdPolicy.Format, NameIdentifierFormats.Transient);
        }

        [Test]
        public void BuildAuthnRequest_test_nameid_fortmat_match_from_many_entries_supported()
        {
            //ARRANGE
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local", NameIdentifierFormats.Persistent);
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient), new Uri(NameIdentifierFormats.Persistent) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContex, supportedNameIdentifierFormats);
            var requestConfiguration = federationContex.GetAuthnRequestConfigurationFromContext(Guid.NewGuid().ToString());
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            //ACT
            var authnRequest = RequestHelper.BuildRequest(authnRequestContext) as AuthnRequest;
            var audience = ((AudienceRestriction)authnRequest.Conditions.Items.Single())
                .Audience
                .Single();

            //ASSERT
            Assert.NotNull(authnRequest);
            Assert.AreEqual(requestConfiguration.IsPassive, authnRequest.IsPassive);
            Assert.AreEqual(requestConfiguration.ForceAuthn, authnRequest.ForceAuthn);
            Assert.AreEqual("2.0", authnRequest.Version);
            //issuer
            Assert.AreEqual(requestConfiguration.EntityId, authnRequest.Issuer.Value);
            Assert.AreEqual(NameIdentifierFormats.Entity, authnRequest.Issuer.Format);
            //audience
            Assert.AreEqual(requestConfiguration.AudienceRestriction.Count, authnRequest.Conditions.Items.Count);
            Assert.AreEqual(requestConfiguration.AudienceRestriction.Single(), audience);
            //nameIdPolicy
            Assert.IsFalse(authnRequest.NameIdPolicy.AllowCreate);
            Assert.AreEqual(authnRequest.NameIdPolicy.Format, NameIdentifierFormats.Persistent);
        }

        [Test]
        public void BuildAuthnRequest_test_nameid_fortmat_no_match_from_many_entries_supported()
        {
            //ARRANGE
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local", NameIdentifierFormats.Windows);
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient), new Uri(NameIdentifierFormats.Persistent) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContex, supportedNameIdentifierFormats);
            var requestConfiguration = federationContex.GetAuthnRequestConfigurationFromContext(Guid.NewGuid().ToString());
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            //ACT
            var authnRequest = RequestHelper.BuildRequest(authnRequestContext) as AuthnRequest;
            var audience = ((AudienceRestriction)authnRequest.Conditions.Items.Single())
                .Audience
                .Single();

            //ASSERT
            Assert.NotNull(authnRequest);
            Assert.AreEqual(requestConfiguration.IsPassive, authnRequest.IsPassive);
            Assert.AreEqual(requestConfiguration.ForceAuthn, authnRequest.ForceAuthn);
            Assert.AreEqual("2.0", authnRequest.Version);
            //issuer
            Assert.AreEqual(requestConfiguration.EntityId, authnRequest.Issuer.Value);
            Assert.AreEqual(NameIdentifierFormats.Entity, authnRequest.Issuer.Format);
            //audience
            Assert.AreEqual(requestConfiguration.AudienceRestriction.Count, authnRequest.Conditions.Items.Count);
            Assert.AreEqual(requestConfiguration.AudienceRestriction.Single(), audience);
            //nameIdPolicy
            Assert.IsFalse(authnRequest.NameIdPolicy.AllowCreate);
            Assert.AreEqual(authnRequest.NameIdPolicy.Format, NameIdentifierFormats.Unspecified);
        }

        [Test]
        public void BuildAuthnRequest_test_nameid_fortmat_no_match_none_supported()
        {
            //ARRANGE
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local", NameIdentifierFormats.Persistent);
            var supportedNameIdentifierFormats = new List<Uri>();
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContex, supportedNameIdentifierFormats);
            var requestConfiguration = federationContex.GetAuthnRequestConfigurationFromContext(Guid.NewGuid().ToString());
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            //ACT
            var authnRequest = RequestHelper.BuildRequest(authnRequestContext) as AuthnRequest;
            var audience = ((AudienceRestriction)authnRequest.Conditions.Items.Single())
                .Audience
                .Single();

            //ASSERT
            Assert.NotNull(authnRequest);
            Assert.AreEqual(requestConfiguration.IsPassive, authnRequest.IsPassive);
            Assert.AreEqual(requestConfiguration.ForceAuthn, authnRequest.ForceAuthn);
            Assert.AreEqual("2.0", authnRequest.Version);
            //issuer
            Assert.AreEqual(requestConfiguration.EntityId, authnRequest.Issuer.Value);
            Assert.AreEqual(NameIdentifierFormats.Entity, authnRequest.Issuer.Format);
            //audience
            Assert.AreEqual(requestConfiguration.AudienceRestriction.Count, authnRequest.Conditions.Items.Count);
            Assert.AreEqual(requestConfiguration.AudienceRestriction.Single(), audience);
            //nameIdPolicy
            Assert.IsFalse(authnRequest.NameIdPolicy.AllowCreate);
            Assert.AreEqual(authnRequest.NameIdPolicy.Format, NameIdentifierFormats.Unspecified);
        }

        [Test]
        public void BuildAuthnRequest_test_nameid_fortmat_encrypted()
        {
            //ARRANGE
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local", NameIdentifierFormats.Encrypted);
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient), new Uri(NameIdentifierFormats.Persistent) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContex, supportedNameIdentifierFormats);
            var requestConfiguration = federationContex.GetAuthnRequestConfigurationFromContext(Guid.NewGuid().ToString());
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            //ACT
            var authnRequest = RequestHelper.BuildRequest(authnRequestContext) as AuthnRequest;
            var audience = ((AudienceRestriction)authnRequest.Conditions.Items.Single())
                .Audience
                .Single();

            //ASSERT
            Assert.NotNull(authnRequest);
            Assert.AreEqual(requestConfiguration.IsPassive, authnRequest.IsPassive);
            Assert.AreEqual(requestConfiguration.ForceAuthn, authnRequest.ForceAuthn);
            Assert.AreEqual("2.0", authnRequest.Version);
            //issuer
            Assert.AreEqual(requestConfiguration.EntityId, authnRequest.Issuer.Value);
            Assert.AreEqual(NameIdentifierFormats.Entity, authnRequest.Issuer.Format);
            //audience
            Assert.AreEqual(requestConfiguration.AudienceRestriction.Count, authnRequest.Conditions.Items.Count);
            Assert.AreEqual(requestConfiguration.AudienceRestriction.Single(), audience);
            //nameIdPolicy
            Assert.IsFalse(authnRequest.NameIdPolicy.AllowCreate);
            Assert.AreEqual(authnRequest.NameIdPolicy.Format, NameIdentifierFormats.Encrypted);
        }
    }
}