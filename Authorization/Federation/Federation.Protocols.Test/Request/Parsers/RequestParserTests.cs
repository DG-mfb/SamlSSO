using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Threading.Tasks;
using DeflateCompression;
using Federation.Metadata.FederationPartner.Handlers;
using Federation.Protocols.Bindings.HttpRedirect;
using Federation.Protocols.Encodiing;
using Federation.Protocols.Request;
using Federation.Protocols.Request.Parsers;
using Federation.Protocols.Request.Validation.ValidationRules;
using Federation.Protocols.Test.Mock;
using Federation.Protocols.Test.Mock.Metadata;
using Kernel.Federation.Protocols;
using NUnit.Framework;
using SecurityManagement;
using Serialisation.Xml;
using Shared.Federtion;
using Shared.Federtion.Factories;

namespace Federation.Protocols.Test.Request.Parsers
{
    [TestFixture]
    internal partial class RequestParserTests
    {
        [Test]
        public async Task ParseAuthnRequest_redirect_binding()
        {
            //ARRANGE
            var authnRequestUrl = await SamlRedirectRequestProviderMock.BuildAuthnRequestRedirectUrl();
            Func<Type, IMetadataHandler> metadataHandlerFactory = t => new MetadataEntitityDescriptorHandler();
            var xmlSerialiser = new XMLSerialiser();
            var compressor = new DeflateCompressor();
            var encoder = new MessageEncoding(compressor);
            var logger = new LogProviderMock();
            var serialiser = new RequestSerialiser(xmlSerialiser, encoder, logger) as IRequestSerialiser;
            var certManager = new CertificateManager(logger);
            Func<IEnumerable<RequestValidationRule>> rulesResolver = () => new[] { new SignatureValidRule(logger, certManager)};
            var requestValidator = new Federation.Protocols.Request.Validation.RequestValidator(logger, new RuleFactory(rulesResolver));
            var configurationRetrieverMock = new ConfigurationRetrieverMock();
            var federationPartyContextBuilderMock = new FederationPartyContextBuilderMock();
            var configurationManger = new ConfigurationManager<MetadataBase>(federationPartyContextBuilderMock, configurationRetrieverMock);
            var requestParser = new RequestParser(metadataHandlerFactory, t => new AuthnRequestParser(serialiser, logger),
                configurationManger, logger, requestValidator);
            var redirectBindingDecoder = new RedirectBindingDecoder(logger, encoder);
            var message = await redirectBindingDecoder.Decode(authnRequestUrl);
            var context = new SamlInboundContext { Message = message };
            //ACT
            var result = await requestParser.Parse(context);
            //ASSERT
            Assert.IsTrue(result.IsValidated);
        }

        [Test]
        public async Task ParseLogoutRequest_redirect_binding()
        {
            //ARRANGE
            var logoutRequestUrl = await SamlRedirectRequestProviderMock.BuildLogoutRequestRedirectUrl();
            Func<Type, IMetadataHandler> metadataHandlerFactory = t => new MetadataEntitityDescriptorHandler();
            var xmlSerialiser = new XMLSerialiser();
            var compressor = new DeflateCompressor();
            var encoder = new MessageEncoding(compressor);
            var logger = new LogProviderMock();
            var serialiser = new RequestSerialiser(xmlSerialiser, encoder, logger) as IRequestSerialiser;
            var certManager = new CertificateManager(logger);
            Func<IEnumerable<RequestValidationRule>> rulesResolver = () => new[] { new SignatureValidRule(logger, certManager) };
            var requestValidator = new Federation.Protocols.Request.Validation.RequestValidator(logger, new RuleFactory(rulesResolver));
            var configurationRetrieverMock = new ConfigurationRetrieverMock();
            var federationPartyContextBuilderMock = new FederationPartyContextBuilderMock();
            var configurationManger = new ConfigurationManager<MetadataBase>(federationPartyContextBuilderMock, configurationRetrieverMock);
            var requestParser = new RequestParser(metadataHandlerFactory, t => new LogoutRequestParser(serialiser, logger),
                configurationManger, logger, requestValidator);
            var redirectBindingDecoder = new RedirectBindingDecoder(logger, encoder);
            var message = await redirectBindingDecoder.Decode(logoutRequestUrl);
            var context = new SamlInboundContext { Message = message };
            //ACT
            var result = await requestParser.Parse(context);
            //ASSERT
            Assert.IsTrue(result.IsValidated);
        }
    }
}