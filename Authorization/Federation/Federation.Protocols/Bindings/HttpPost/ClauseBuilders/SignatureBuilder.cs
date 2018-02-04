using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Federation.Protocols.Tokens;
using Kernel.Cryptography.Signing.Xml;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;
using Kernel.Logging;
using Kernel.Security.CertificateManagement;

namespace Federation.Protocols.Bindings.HttpPost.ClauseBuilders
{
    internal class SignatureBuilder : IPostClauseBuilder
    {
        private readonly ICertificateManager _certificateManager;
        readonly ILogProvider _logProvider;
        private readonly IXmlSignatureManager _xmlSignatureManager;

        public SignatureBuilder(ICertificateManager certificateManager, ILogProvider logProvider, IXmlSignatureManager xmlSignatureManager)
        {
            this._certificateManager = certificateManager;
            this._logProvider = logProvider;
            this._xmlSignatureManager = xmlSignatureManager;
        }
        public uint Order { get { return 2; } }

        public Task Build(BindingContext context)
        {

            if (context == null)
                throw new ArgumentNullException("context");

            var requestContext = ((RequestPostBindingContext)context).RequestContext;
            
            var descriptor = requestContext.FederationPartyContext.MetadataContext.EntityDesriptorConfiguration.SPSSODescriptors
                .First();
            if (descriptor.AuthenticationRequestsSigned)
            {
                this._logProvider.LogMessage("Signing authentication request.");
                var certContext = descriptor.KeyDescriptors
                    .First(k => k.Use == Kernel.Federation.MetaData.Configuration.Cryptography.KeyUsage.Signing).CertificateContext;
                var cert = this._certificateManager.GetCertificateFromContext(certContext);
                var request = context.RequestParts[HttpRedirectBindingConstants.SamlRequest];
                var document = new XmlDocument();
                document.LoadXml(request);
                this._xmlSignatureManager.WriteSignature(document, requestContext.RequestId, cert.PrivateKey, "", "");
                var signature = TokenHelper.GetElement("Signature", "http://www.w3.org/2000/09/xmldsig#", document.DocumentElement);
                if (signature == null)
                    throw new InvalidOperationException("Signature element not present.");
                signature.ParentNode.RemoveChild(signature);
                var issuer = TokenHelper.GetElement("Issuer", Saml20Constants.Assertion, document.DocumentElement);
                if (issuer == null)
                    throw new InvalidOperationException("Issuer element not present.");
                issuer.ParentNode.InsertAfter(signature, issuer);
                this._logProvider.LogMessage(String.Format("Authentication request signed./r/n{0}", document.OuterXml));
                context.RequestParts[HttpRedirectBindingConstants.SamlRequest] = document.OuterXml;
            }
            return Task.CompletedTask;
        }
    }
}