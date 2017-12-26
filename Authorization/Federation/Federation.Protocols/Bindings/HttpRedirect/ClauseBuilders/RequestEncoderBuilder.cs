using System;
using System.Threading.Tasks;
using Federation.Protocols.Request;
using Kernel.Federation.Protocols;
using Shared.Federtion.Constants;

namespace Federation.Protocols.Bindings.HttpRedirect.ClauseBuilders
{
    internal class RequestEncoderBuilder : IRedirectClauseBuilder
    {
        private readonly IMessageEncoding _messageEncoding;
        public RequestEncoderBuilder(IMessageEncoding messageEncoding)
        {
            this._messageEncoding = messageEncoding;
        }
        public uint Order { get { return 1; } }

        public async Task Build(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var authnRequest = context.RequestParts[HttpRedirectBindingConstants.SamlRequest];
            var compressed = await this._messageEncoding.EncodeMessage(authnRequest);
            context.RequestParts[HttpRedirectBindingConstants.SamlRequest] = compressed;
        }
    }
}