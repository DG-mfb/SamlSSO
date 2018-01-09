using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using DeflateCompression;
using Federation.Metadata.FederationPartner.Handlers;
using Federation.Protocols.Bindings.HttpPost;
using Federation.Protocols.Encodiing;
using Federation.Protocols.RelayState;
using Federation.Protocols.Response;
using Federation.Protocols.Response.Validation.ValidationRules;
using Federation.Protocols.Test.Mock;
using Federation.Protocols.Test.Mock.Metadata;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;
using NUnit.Framework;
using SecurityManagement;
using SecurityManagement.Signing;
using Serialisation.JSON;
using Serialisation.JSON.SettingsProviders;
using Serialisation.Xml;
using Shared.Federtion;
using Shared.Federtion.Factories;
using Shared.Federtion.Forms;

namespace Federation.Protocols.Test.Response.Parsers
{
    [TestFixture]
    internal partial class PostResponseParserTests
    {
        [Test]
        public async Task ParseTokenResponse_post_binding()
        {
            //ARRANGE
            var inResponseTo = Guid.NewGuid().ToString();

            var response = ResponseFactoryMock.GetTokenResponseSuccess(inResponseTo, StatusCodes.Success);
            var logger = new LogProviderMock();
            var serialised = ResponseFactoryMock.Serialize(response);
            var xmlSignatureManager = new XmlSignatureManager();
            var document = new XmlDocument();
            document.LoadXml(serialised);
            var cert = AssertionFactroryMock.GetMockCertificate();
            xmlSignatureManager.SignXml(document, response.ID, cert.PrivateKey);
            var base64Encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(document.DocumentElement.OuterXml));

            var compressor = new DeflateCompressor();
            var encoder = new MessageEncoding(compressor);
            var jsonSerialiser = new NSJsonSerializer(new DefaultSettingsProvider());
            var relayStateSerialiser = new RelaystateSerialiser(jsonSerialiser, encoder, logger) as IRelayStateSerialiser;
            var relayState = await relayStateSerialiser.Serialize(new Dictionary<string, object> { { "Key", "Value" } });
            
            var form = new SAMLForm();
            form.SetResponse(base64Encoded);
            form.SetRelatState(relayState);

            Func<Type, IMetadataHandler> metadataHandlerFactory = t => new MetadataEntitityDescriptorHandler();
            var xmlSerialiser = new XMLSerialiser();
            
            var certManager = new CertificateManager(logger);
            var signatureManager = new XmlSignatureManager();
            Func<IEnumerable<ResponseValidationRule>> rulesResolver = () => new[] { new ResponseSignatureRule(logger, certManager, signatureManager)};
            var requestValidator = new Federation.Protocols.Response.Validation.ResponseValidator(logger, new RuleFactory(rulesResolver));
            var configurationRetrieverMock = new ConfigurationRetrieverMock();
            var federationPartyContextBuilderMock = new FederationPartyContextBuilderMock();
            var configurationManger = new ConfigurationManager<MetadataBase>(federationPartyContextBuilderMock, configurationRetrieverMock);
            var relayStateHandler = new RelayStateHandler(relayStateSerialiser, logger);
            var responseParser = new ResponseParser(metadataHandlerFactory, t => new SamlTokenResponseParser(logger),
                configurationManger, relayStateHandler, logger, requestValidator);
            var postBindingDecoder = new PostBindingDecoder(logger);
            var message = await postBindingDecoder.Decode(form.HiddenControls.ToDictionary(k => k.Key, v => v.Value));
            var context = new SamlInboundContext { Message = message };
            //ACT
            var result = await responseParser.Parse(context);
            //ASSERT
            Assert.IsTrue(result.IsValidated);
        }

        [Test]
        public async Task ParseLogoutResponse_post_binding()
        {
            throw new NotImplementedException();
           
        }
    }
}