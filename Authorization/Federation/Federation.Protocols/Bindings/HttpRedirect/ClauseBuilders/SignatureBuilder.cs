using System;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpRedirectBinding;
using Kernel.Logging;
using Kernel.Security.CertificateManagement;

namespace Federation.Protocols.Bindings.HttpRedirect.ClauseBuilders
{
    internal class SignatureBuilder : IRedirectClauseBuilder
    {
        private readonly ICertificateManager _certificateManager;
        readonly ILogProvider _logProvider;

        public SignatureBuilder(ICertificateManager certificateManager, ILogProvider logProvider)
        {
            this._certificateManager = certificateManager;
            this._logProvider = logProvider;
        }
        public uint Order { get { return 3; } }

        public Task Build(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var httpRedirectContext = context as RequestBindingContext;
            if (httpRedirectContext == null)
                throw new InvalidOperationException(String.Format("Binding context must be of type:{0}. It was: {1}", typeof(RequestBindingContext).Name, context.GetType().Name));
            var federationParty = httpRedirectContext.RequestContext.FederationPartyContext;
            var metadataContext = federationParty.MetadataContext;
            var entityDescriptor = metadataContext.EntityDesriptorConfiguration;
            var spDescriptor = entityDescriptor.SPSSODescriptors
                .First();
            var certContext = spDescriptor.KeyDescriptors.First(x => x.IsDefault && x.Use == Kernel.Federation.MetaData.Configuration.Cryptography.KeyUsage.Signing)
                .CertificateContext;
            if (spDescriptor.AuthenticationRequestsSigned)
                this.SignRequest(httpRedirectContext, certContext);
            return Task.CompletedTask;
        }

        internal void SignRequest(BindingContext context, CertificateContext certContext)
        {
            context.RequestParts.Add(HttpRedirectBindingConstants.SigAlg, SignedXml.XmlDsigRSASHA1Url);
            this.SignData((HttpRedirectContext)context, certContext);
        }
        internal void SignData(HttpRedirectContext context, CertificateContext certContext)
        {
            var query = context.BuildQuesryString();
            this._logProvider.LogMessage(String.Format("Signing request with certificate from context: {0}", certContext.ToString()));
            var base64 = this._certificateManager.SignToBase64(query, certContext);
            context.RequestParts.Add(HttpRedirectBindingConstants.Signature, base64);
        }
    }
}