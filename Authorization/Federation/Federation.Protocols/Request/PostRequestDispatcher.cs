using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Kernel.Cryptography.Signing.Xml;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using Kernel.Logging;
using Kernel.Security.CertificateManagement;
using Shared.Federtion.Forms;

namespace Federation.Protocols.Request
{
    internal class PostRequestDispatcher : ISamlMessageDespatcher<HttpPostRequestContext>
    {
        private readonly IAuthnRequestSerialiser _serialiser;
        private readonly ILogProvider _logProvider;
        private IRelayStateSerialiser _relayStateSerialiser;
        private ICertificateManager _certManager;
        private IXmlSignatureManager _xmlSignatureManager;

        public PostRequestDispatcher(IAuthnRequestSerialiser serialiser, IRelayStateSerialiser relayStateSerialiser, IXmlSignatureManager xmlSignatureManager, ICertificateManager certManager, ILogProvider logProvider)
        {
            this._xmlSignatureManager = xmlSignatureManager;
            this._certManager = certManager;
            this._serialiser = serialiser;
            this._logProvider = logProvider;
            this._relayStateSerialiser = relayStateSerialiser;
        }
        public Task SendAsync(SamlOutboundContext context)
        {
            return this.SendAsync(context as HttpPostRequestContext);
        }

        public async Task SendAsync(HttpPostRequestContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            var requestContext = ((RequestBindingContext)context.BindingContext).AuthnRequestContext;
            var request = AuthnRequestHelper.BuildAuthnRequest(requestContext);

            var serialised = this._serialiser.Serialize(request);
            var document = new XmlDocument();
            this._logProvider.LogMessage(String.Format("Authentication request serialised./r/n{0}", serialised));
            document.LoadXml(serialised);
            var descriptor = requestContext.FederationPartyContext.MetadataContext.EntityDesriptorConfiguration.SPSSODescriptors
                .First();
            if (descriptor.AuthenticationRequestsSigned)
            {
                this._logProvider.LogMessage("Signing authn request.");
                var certContext = descriptor.KeyDescriptors
                    .First(k => k.Use == Kernel.Federation.MetaData.Configuration.Cryptography.KeyUsage.Signing).CertificateContext;
                var cert = this._certManager.GetCertificateFromContext(certContext);
                this._xmlSignatureManager.WriteSignature(document, requestContext.FederationPartyContext.MetadataContext.EntityDesriptorConfiguration.Id, cert.PrivateKey, "", "");
                this._logProvider.LogMessage(String.Format("Authentication request signed./r/n{0}", document.OuterXml));
            }
            var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(document.OuterXml));

            var relyingStateSerialised = await this._relayStateSerialiser.Serialize(context.BindingContext.RelayState);
            this._logProvider.LogMessage(String.Format("Building SAML form. Destination url: {0}", context.BindingContext.DestinationUri.AbsoluteUri));
            var form = new SAMLForm
            {
                ActionURL = context.BindingContext.DestinationUri.AbsoluteUri
            };
            form.AddHiddenControl("SAMLRequest", base64Encoded);
            form.AddHiddenControl("RelayState", relyingStateSerialised);
            var samlForm = form.ToString();
            this._logProvider.LogMessage(String.Format("Despatching saml form./r/n. {0}", samlForm));
            await context.DespatchDelegate(samlForm);
        }
    }
}