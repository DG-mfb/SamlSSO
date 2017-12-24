using System;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Kernel.Cryptography.Signing.Xml;
using Kernel.Federation.Protocols;
using Kernel.Logging;
using Kernel.Security.CertificateManagement;
using Shared.Federtion.Constants;

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

            var requestContext = ((RequestBindingContext)context).AuthnRequestContext;
            
            var descriptor = requestContext.FederationPartyContext.MetadataContext.EntityDesriptorConfiguration.SPSSODescriptors
                .First();
            if (descriptor.AuthenticationRequestsSigned)
            {
                this._logProvider.LogMessage("Signing authentication request.");
                var certContext = descriptor.KeyDescriptors
                    .First(k => k.Use == Kernel.Federation.MetaData.Configuration.Cryptography.KeyUsage.Signing).CertificateContext;
                var cert = this._certificateManager.GetCertificateFromContext(certContext);
                this._xmlSignatureManager.WriteSignature(context.Request, requestContext.RequestId, cert.PrivateKey, "", "");
                this._logProvider.LogMessage(String.Format("Authentication request signed./r/n{0}", context.Request.OuterXml));
            }
            return Task.CompletedTask;
        }

        internal void SignRequest(StringBuilder sb, CertificateContext certContext)
        {
            this.AppendSignarureAlgorithm(sb);
            this.SignData(sb, certContext);
        }
        internal void SignData(StringBuilder sb, CertificateContext certContext)
        {
            this._logProvider.LogMessage(String.Format("Signing request with certificate from context: {0}", certContext.ToString()));
            var base64 = this._certificateManager.SignToBase64(sb.ToString(), certContext);
            var escaped = Uri.EscapeDataString(Helper.UpperCaseUrlEncode(base64));
            sb.AppendFormat("&{0}={1}", HttpRedirectBindingConstants.Signature, escaped);
        }

        internal void AppendSignarureAlgorithm(StringBuilder builder)
        {
            builder.AppendFormat("&{0}={1}", HttpRedirectBindingConstants.SigAlg, Uri.EscapeDataString(SignedXml.XmlDsigRSASHA1Url));
        }
    }
}