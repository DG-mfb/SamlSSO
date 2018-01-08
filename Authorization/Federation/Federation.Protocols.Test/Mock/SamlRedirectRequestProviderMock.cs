using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeflateCompression;
using Federation.Protocols.Bindings.HttpRedirect;
using Federation.Protocols.Bindings.HttpRedirect.ClauseBuilders;
using Federation.Protocols.Encodiing;
using Federation.Protocols.RelayState;
using Federation.Protocols.Request;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Request;
using SecurityManagement;
using Serialisation.JSON;
using Serialisation.JSON.SettingsProviders;
using Serialisation.Xml;

namespace Federation.Protocols.Test.Mock
{
    internal class SamlRedirectRequestProviderMock
    {
        public static async Task<Uri> BuildAuthnRequestRedirectUrl()
        {
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local");
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContex, supportedNameIdentifierFormats);
            var bindingContext = await SamlRedirectRequestProviderMock.BuildRequestBindingContext(authnRequestContext);
            return bindingContext.GetDestinationUrl();
        }

        public static async Task<Uri> BuildLogoutRequestRedirectUrl()
        {
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local");
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new LogoutRequestContext(requestUri, new Uri("http://localhost"), federationContex, new Uri(Reasons.User));
            var bindingContext = await SamlRedirectRequestProviderMock.BuildRequestBindingContext(authnRequestContext);
            return bindingContext.GetDestinationUrl();
        }

        public static Task<SamlInboundMessage> BuilSamlInboundMessage()
        {
            throw new NotImplementedException();
        }

        public static async Task<RequestBindingContext> BuildRequestBindingContext(RequestContext requestContext)
        {
            string url = String.Empty;
            var builders = new List<IRedirectClauseBuilder>();
            
            requestContext.RelyingState.Add("relayState", "Test state");
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
            var bindingContext = new RequestBindingContext(requestContext);
            foreach (var b in builders)
            {
                await b.Build(bindingContext);
            }

            return bindingContext;
        }
    }
}
