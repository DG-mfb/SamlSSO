using System;
using System.Collections.Concurrent;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.MetaData;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpRedirectBinding;
using Kernel.Security.CertificateManagement;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request.Handlers
{
    internal class AuthnRequestHandler
    {
        private static ConcurrentDictionary<string, Uri> _relyingParties = new ConcurrentDictionary<string, Uri>();

        private readonly  IRelayStateHandler _relayStateHandler;
        private readonly ICertificateManager _certificateManager;
        private readonly IAuthnRequestSerialiser _authnRequestSerialiser;
        private readonly IConfigurationRetriever<MetadataBase> _configurationRetriever;
        private readonly IMetadataHandler<EntityDescriptor> _metadataHandler;
        public AuthnRequestHandler(IRelayStateHandler relayStateHandler, 
            ICertificateManager certificateManager,
            IAuthnRequestSerialiser authnRequestSerialiser,
            IConfigurationRetriever<MetadataBase> configurationRetriever,
            IMetadataHandler<EntityDescriptor> metadataHandler)
        {
            this._metadataHandler = metadataHandler;
            this._configurationRetriever = configurationRetriever;
            this._authnRequestSerialiser = authnRequestSerialiser;
            this._relayStateHandler = relayStateHandler;
            this._certificateManager = certificateManager;
            AuthnRequestHandler._relyingParties.TryAdd("https://imperial.flowz_test.co.uk/", new Uri("http://localhost:60879/sp/metadata"));
        }
        public async Task HandleRequest(HttpRedirectInboundContext context)
        {
            var requestEncoded = context.Form["SAMLRequest"];
            var relayState = await this._relayStateHandler.GetRelayStateFromFormData(context.Form);
            var request = await this._authnRequestSerialiser.Deserialize<AuthnRequest>(requestEncoded);
            var spDescriptor = await this.GetSPDescriptor(request);
            var keyDescriptors = spDescriptor.Keys.Where(k => k.Use == KeyType.Signing);
            var validated = false;
            foreach (var k in keyDescriptors.SelectMany(x => x.KeyInfo))
            {
                var binaryClause = k as BinaryKeyIdentifierClause;
                if (binaryClause == null)
                    throw new InvalidOperationException(String.Format("Expected type: {0} but it was: {1}", typeof(BinaryKeyIdentifierClause), k.GetType()));

                var certContent = binaryClause.GetBuffer();
                var cert = new X509Certificate2(certContent);
                validated = this.VerifySignature(context.Request, cert, this._certificateManager);
                if (validated)
                    break;
            }
            if (!validated)
                throw new InvalidOperationException("Invalid signature.");
            context.HanlerAction();
        }

        private async Task<ServiceProviderSingleSignOnDescriptor> GetSPDescriptor(AuthnRequest request)
        {
            var issuer = request.Issuer.Value;
            if (!AuthnRequestHandler._relyingParties.ContainsKey(issuer))
                throw new InvalidOperationException(String.Format("Unregistered relying party id: {0}", issuer));
            var issuerMetadataLocation = AuthnRequestHandler._relyingParties[issuer];
            
            var spMetadata = await this._configurationRetriever.GetAsync(new FederationPartyConfiguration("local", issuerMetadataLocation.AbsoluteUri), CancellationToken.None);
            var metadataType = spMetadata.GetType();
            var handlerType = typeof(IMetadataHandler<>).MakeGenericType(metadataType);
            //var handler = resolver.Resolve(handlerType) as IMetadataHandler<EntityDescriptor>;
            //if (handler == null)
            //    throw new InvalidOperationException(String.Format("Handler must implement: {0}", typeof(IMetadataHandler).Name));
            var spDescriptor = this._metadataHandler.GetRoleDescriptors<ServiceProviderSingleSignOnDescriptor>((EntityDescriptor)spMetadata)
                .Single();
            return spDescriptor;
        }
        private bool VerifySignature(string request, X509Certificate2 certificate, ICertificateManager certificateManager)
        {
            var i = request.IndexOf("Signature");
            var data = request.Substring(0, i - 1);
            var sgn = Uri.UnescapeDataString(request.Substring(i + 10));


            var validated = certificateManager.VerifySignatureFromBase64(data, sgn, certificate);
            return validated;
        }
    }
}
