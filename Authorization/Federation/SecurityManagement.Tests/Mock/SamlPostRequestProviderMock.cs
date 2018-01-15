using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeflateCompression;
using Federation.Protocols.Bindings.HttpPost;
using Federation.Protocols.Bindings.HttpPost.ClauseBuilders;
using Federation.Protocols.Encodiing;
using Federation.Protocols.RelayState;
using Federation.Protocols.Request;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Request;
using SecurityManagement.Signing;
using Serialisation.JSON;
using Serialisation.JSON.SettingsProviders;
using Serialisation.Xml;
using Shared.Federtion.Forms;

namespace SecurityManagement.Tests.Mock
{
    internal class SamlPostRequestProviderMock
    {
        public static async Task<SAMLForm> BuildAuthnRequestPostForm()
        {
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local");
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContex, supportedNameIdentifierFormats);
            var form = await SamlPostRequestProviderMock.BuildRequestBindingContext(authnRequestContext);
            return form;
        }
        public static async Task<Tuple<string, string>> BuildAuthnRequest()
        {
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local");
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var authnRequestContext = new AuthnRequestContext(requestUri, new Uri("http://localhost"), federationContex, supportedNameIdentifierFormats);
            var request = await SamlPostRequestProviderMock.BuildRequest(authnRequestContext);

            return new Tuple<string, string>(request, authnRequestContext.RequestId);
        }

        public static async Task<string> BuildLogoutRequest()
        {
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local");
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var logoutContext = new SamlLogoutContext(new Uri(Reasons.User), new System.IdentityModel.Tokens.Saml2NameIdentifier("testUser"));
            var authnRequestContext = new LogoutRequestContext(requestUri, new Uri("http://localhost"), federationContex, logoutContext);
            var request = await SamlPostRequestProviderMock.BuildRequest(authnRequestContext);
            return request;
        }

        public static async Task<SAMLForm> BuildLogoutRequestPostForm()
        {
            var requestUri = new Uri("http://localhost:59611/");
            var federationPartyContextBuilder = new FederationPartyContextBuilderMock();
            var federationContex = federationPartyContextBuilder.BuildContext("local");
            var supportedNameIdentifierFormats = new List<Uri> { new Uri(NameIdentifierFormats.Transient) };
            var logoutContext = new SamlLogoutContext(new Uri(Reasons.User), new System.IdentityModel.Tokens.Saml2NameIdentifier("testUser"));
            var authnRequestContext = new LogoutRequestContext(requestUri, new Uri("http://localhost"), federationContex, logoutContext);
            var form = await SamlPostRequestProviderMock.BuildRequestBindingContext(authnRequestContext);
            return form;
        }

        public static async Task<SAMLForm> BuildRequestBindingContext(RequestContext requestContext)
        {
            string url = String.Empty;
            var builders = new List<IPostClauseBuilder>();

            requestContext.RelyingState.Add("relayState", "Test state");
            var xmlSerialiser = new XMLSerialiser();
            var compressor = new DeflateCompressor();
            var encoder = new MessageEncoding(compressor);
            var logger = new LogProviderMock();
            var serialiser = new RequestSerialiser(xmlSerialiser, encoder, logger);
            var xmlSinatureManager = new XmlSignatureManager();
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            var authnBuilder = new SamlRequestBuilder(serialiser);
            builders.Add(authnBuilder);

            //relay state builder
            var jsonSerialiser = new NSJsonSerializer(new DefaultSettingsProvider());
            var relayStateSerialiser = new RelaystateSerialiser(jsonSerialiser, encoder, logger) as IRelayStateSerialiser;
            var relayStateBuilder = new RelayStateBuilder(relayStateSerialiser);
            builders.Add(relayStateBuilder);

            //signature builder
            var certificateManager = new CertificateManager(logger);
            var signatureBuilder = new SignatureBuilder(certificateManager, logger, xmlSinatureManager);
            builders.Add(signatureBuilder);
            var bindingContext = new RequestPostBindingContext(requestContext);
            foreach (var b in builders)
            {
                await b.Build(bindingContext);
            }
            var form = new SAMLForm();
            var request = bindingContext.RequestParts[HttpRedirectBindingConstants.SamlRequest];
            var base64Encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request));

            var relyingStateSerialised = bindingContext.RequestParts[HttpRedirectBindingConstants.RelayState];

            form.ActionURL = bindingContext.DestinationUri.AbsoluteUri;
            form.SetRequest(base64Encoded);
            form.SetRelatState(relyingStateSerialised);

            return form;
        }

        public static async Task<string> BuildRequest(RequestContext requestContext)
        {
            string url = String.Empty;
            var builders = new List<IPostClauseBuilder>();

            requestContext.RelyingState.Add("relayState", "Test state");
            var xmlSerialiser = new XMLSerialiser();
            var compressor = new DeflateCompressor();
            var encoder = new MessageEncoding(compressor);
            var logger = new LogProviderMock();
            var serialiser = new RequestSerialiser(xmlSerialiser, encoder, logger);
            var xmlSinatureManager = new XmlSignatureManager();
            RequestHelper.GetAuthnRequestBuilders = AuthnRequestBuildersFactoryMock.GetAuthnRequestBuildersFactory();
            var authnBuilder = new SamlRequestBuilder(serialiser);
            builders.Add(authnBuilder);

            //relay state builder
            var jsonSerialiser = new NSJsonSerializer(new DefaultSettingsProvider());
            var relayStateSerialiser = new RelaystateSerialiser(jsonSerialiser, encoder, logger) as IRelayStateSerialiser;
            var relayStateBuilder = new RelayStateBuilder(relayStateSerialiser);
            builders.Add(relayStateBuilder);

            //signature builder
            var certificateManager = new CertificateManager(logger);
            var signatureBuilder = new SignatureBuilder(certificateManager, logger, xmlSinatureManager);
            builders.Add(signatureBuilder);
            var bindingContext = new RequestPostBindingContext(requestContext);
            foreach (var b in builders)
            {
                await b.Build(bindingContext);
            }
            var form = new SAMLForm();
            var request = bindingContext.RequestParts[HttpRedirectBindingConstants.SamlRequest];

            return request;
        }
    }
}