using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeflateCompression;
using Federation.Protocols.Bindings.HttpRedirect;
using Federation.Protocols.Bindings.HttpRedirect.ClauseBuilders;
using Federation.Protocols.Encodiing;
using Federation.Protocols.RelayState;
using Federation.Protocols.Request;
using Federation.Protocols.Test.Mock;
using Kernel.Federation.Constants;
using Kernel.Federation.MetaData.Configuration.Cryptography;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Request;
using NUnit.Framework;
using SecurityManagement;
using Serialisation.JSON;
using Serialisation.JSON.SettingsProviders;
using Serialisation.Xml;

namespace Federation.Protocols.Test.Encoding
{
    [TestFixture]
    internal class RedirectBindingDecodingTest
    {
        [Test]
        public async Task DecodeTest()
        {
            string url = String.Empty;
            var builders = new List<IRedirectClauseBuilder>();

            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local");
            var spDescriptor = federationContex.MetadataContext.EntityDesriptorConfiguration.SPSSODescriptors.First();
            var certContext = spDescriptor.KeyDescriptors.Where(x => x.Use == KeyUsage.Signing && x.IsDefault)
                .Select(x => x.CertificateContext)
                .First();
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContex, supportedNameIdentifierFormats);
            authnRequestContext.RelyingState.Add("relayState", "Test state");
            var xmlSerialiser = new XMLSerialiser();
            var compressor = new DeflateCompressor();
            var encoder = new MessageEncoding(compressor);
            var logger = new LogProviderMock();
            var serialiser = new RequestSerialiser(xmlSerialiser, encoder, logger);
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            var authnBuilder = new SamlRequestBuilder(serialiser);
            builders.Add(authnBuilder);

            //request compression builder
            var encodingBuilder = new RequestEncoderBuilder(encoder);
            builders.Add(encodingBuilder);

            //relay state builder
            var jsonSerialiser = new NSJsonSerializer(new DefaultSettingsProvider());
            var relayStateSerialiser = new RelaystateSerialiser(jsonSerialiser, encoder, logger) as IRelayStateSerialiser;
            var relayStateBuilder = new RelayStateBuilder(relayStateSerialiser);
            builders.Add(relayStateBuilder);

            //signature builder
            var certificateManager = new CertificateManager(logger);
            var signatureBuilder = new SignatureBuilder(certificateManager, logger);
            builders.Add(signatureBuilder);
            var bindingContext = new RequestBindingContext(authnRequestContext);
            foreach(var b in builders)
            {
                await b.Build(bindingContext);
            }
            var decoder = new RedirectBindingDecoder(logger, encoder);
            //ACT
            var message = await decoder.Decode(bindingContext.GetDestinationUrl());
            var stateFromResult = message.Elements[HttpRedirectBindingConstants.RelayState];
            var requestFromContext = bindingContext.RequestParts[HttpRedirectBindingConstants.SamlRequest];
            var decoded = await encoder.DecodeMessage(requestFromContext);
            //ASSERT
            Assert.IsNotNull(stateFromResult);
            Assert.AreEqual(bindingContext.RequestParts[HttpRedirectBindingConstants.RelayState], message.Elements[HttpRedirectBindingConstants.RelayState]);
            Assert.AreEqual(decoded, message.Elements[HttpRedirectBindingConstants.SamlRequest]);
        }
    }
}
