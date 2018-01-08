using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Federation.Protocols.Response.Validation;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;
using Kernel.Logging;
using Kernel.Reflection;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class ResponseParser : IMessageParser<SamlInboundContext, SamlInboundResponseContext>
    {
        private readonly ILogProvider _logProvider;
        private readonly ResponseValidator _responseValidator;
        private readonly Func<Type, SamlResponseParser> _samlResponseParserFactory;
        private readonly MessageTypeResolver _messageTypeResolver = new MessageTypeResolver();
        private readonly IRelayStateHandler _relayStateHandler;
        public ResponseParser(Func<Type, SamlResponseParser> samlResponseParserFactory, IRelayStateHandler relayStateHandler, ILogProvider logProvider, ResponseValidator responseValidator)
        {
            this._samlResponseParserFactory = samlResponseParserFactory;
            this._logProvider = logProvider;
            this._responseValidator = responseValidator;
            this._relayStateHandler = relayStateHandler;
        }
        public async Task<SamlInboundResponseContext> Parse(SamlInboundContext context)
        {
            var message = context.Message;
            var responseText = message.SamlMessage;
            
            var responseTypes = this.GetTypes();
            var type = this._messageTypeResolver.ResolveMessageType(responseText, responseTypes);
            var statusResponse =  this._samlResponseParserFactory(type).Parse(responseText);
            
            var relayState = await this.ResolveRelayState(message, !String.IsNullOrEmpty(statusResponse.InResponseTo));
            var responseContext = new SamlInboundResponseContext { StatusResponse = statusResponse, SamlInboundMessage = message };
            await this._responseValidator.ValidateResponse(responseContext);
            return responseContext;
        }

        public IEnumerable<Type> GetTypes()
        {
            var types = ReflectionHelper.GetAllTypes(t => !t.IsAbstract && !t.IsInterface && typeof(StatusResponse).IsAssignableFrom(t));
            return types;
        }

        private async Task<object> ResolveRelayState(SamlInboundMessage message, bool spInitiated)
        {
            if (!message.HasRelaySate)
                return null;

            var relayStateRaw = message.RelayState;
            object relayState = relayStateRaw;
            if (spInitiated && relayStateRaw != null)
                relayState = await this._relayStateHandler.Decode(relayStateRaw.ToString());
            message.Elements[HttpRedirectBindingConstants.RelayState] = relayState;
            return relayState;
        }
    }
}