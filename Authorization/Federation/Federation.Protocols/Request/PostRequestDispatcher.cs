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

            var request = AuthnRequestHelper.BuildAuthnRequest(context.BindingContext.AuthnRequestContext);

            var serialised = this._serialiser.Serialize(request);
            var document = new XmlDocument();
            document.LoadXml(serialised);
            var descriptor = context.BindingContext.AuthnRequestContext.FederationPartyContext.MetadataContext.EntityDesriptorConfiguration.SPSSODescriptors
                .First();
            if (descriptor.AuthenticationRequestsSigned)
            {
                var certContext = descriptor.KeyDescriptors
                    .First(k => k.Use == Kernel.Federation.MetaData.Configuration.Cryptography.KeyUsage.Signing).CertificateContext;
                var cert = this._certManager.GetCertificateFromContext(certContext);
                this._xmlSignatureManager.WriteSignature(document, context.BindingContext.AuthnRequestContext.FederationPartyContext.MetadataContext.EntityDesriptorConfiguration.Id, cert.PrivateKey, "", "");
            }
            var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(document.OuterXml));

            var relyingStateSerialised = await this._relayStateSerialiser.Serialize(context.BindingContext.RelayState);
            var form = new SAMLForm
            {
                ActionURL = context.BindingContext.DestinationUri.AbsoluteUri
            };
            form.AddHiddenControl("SAMLRequest", base64Encoded);
            form.AddHiddenControl("RelayState", relyingStateSerialised);
            var samlForm = form.ToString();
            await context.DespatchDelegate(samlForm);
        }
    }
}