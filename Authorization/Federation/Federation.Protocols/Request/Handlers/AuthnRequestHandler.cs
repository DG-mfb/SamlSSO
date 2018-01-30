using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpRedirectBinding;
using Shared.Federtion.Models;
using Shared.Federtion.Request;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace Federation.Protocols.Request.Handlers
{
    internal class AuthnRequestHandler : IInboundHandler<HttpRedirectInboundContext>
    {
        private static ConcurrentDictionary<string, Uri> _relyingParties = new ConcurrentDictionary<string, Uri>();
        static AuthnRequestHandler()
        {
            AuthnRequestHandler._relyingParties.TryAdd("https://sso.flowz-dev-local.co.uk/", new Uri("http://localhost:60879/sp/metadata"));
            AuthnRequestHandler._relyingParties.TryAdd("https://www.eca-international-local.com", new Uri("http://localhost:60879/sp/metadata"));
        }

        private readonly IMessageParser<SamlInboundContext, SamlInboundRequestContext> _messageParser;
        public AuthnRequestHandler(IMessageParser<SamlInboundContext, SamlInboundRequestContext> messageParser)
        {
            this._messageParser = messageParser;
        }
        public async Task Handle(HttpRedirectInboundContext context)
        {
            var requestContext = await this._messageParser.Parse(context);
            if (requestContext.IsValidated)
                context.HanlerAction();
            //throw new NotImplementedException();
            //var requestEncoded = context.Form["SAMLRequest"];
            //var relayState = await this._relayStateHandler.GetRelayStateFromFormData(context.Form);
            //var decompressed = await this._authnRequestSerialiser.Decompress(requestEncoded);
            //var type = this.ResolveRequestType(decompressed);
            //var request = this._authnRequestSerialiser.Deserialize<AuthnRequest>(decompressed);
            //var spDescriptor = await this.GetSPDescriptor(request);
            //var keyDescriptors = spDescriptor.Keys.Where(k => k.Use == KeyType.Signing);
            //var validated = false;
            //foreach (var k in keyDescriptors.SelectMany(x => x.KeyInfo))
            //{
            //    var binaryClause = k as BinaryKeyIdentifierClause;
            //    if (binaryClause == null)
            //        throw new InvalidOperationException(String.Format("Expected type: {0} but it was: {1}", typeof(BinaryKeyIdentifierClause), k.GetType()));

            //    var certContent = binaryClause.GetBuffer();
            //    var cert = new X509Certificate2(certContent);
            //    validated = this.VerifySignature(context.Request, cert, this._certificateManager);
            //    if (validated)
            //        break;
            //}
            //if (!validated)
            //    throw new InvalidOperationException("Invalid signature.");
            //context.HanlerAction();
        }

        private Type ResolveRequestType(string message)
        {
            using (var reader = XmlReader.Create(new StringReader(message)))
            {
                reader.MoveToContent();
                if(reader.IsStartElement(AuthnRequest.ElementName,Saml20Constants.Protocol))
                    return typeof(AuthnRequest);
                throw new NotSupportedException();
            }
        }
    }
}
