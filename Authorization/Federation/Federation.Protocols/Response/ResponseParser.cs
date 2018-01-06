using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Federation.Protocols.Response.Validation;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Response;
using Kernel.Logging;
using Kernel.Reflection;
using Shared.Federtion.Constants;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class ResponseParser : IResponseParser<SamlInboundContext, SamlResponseContext>
    {
        private readonly ILogProvider _logProvider;
        private readonly ResponseValidator _responseValidator;
        private readonly Func<Type, SamlResponseParser> _samlResponseParserFactory;
        private readonly IRelayStateHandler _relayStateHandler;
        private readonly MessageTypeResolver _messageTypeResolver = new MessageTypeResolver();
        public ResponseParser(Func<Type, SamlResponseParser> samlResponseParserFactory, ILogProvider logProvider, IRelayStateHandler relayStateHandler, ResponseValidator responseValidator)
        {
            this._samlResponseParserFactory = samlResponseParserFactory;
            this._relayStateHandler = relayStateHandler;
            this._logProvider = logProvider;
            this._responseValidator = responseValidator;
        }
        public async Task<SamlResponseContext> ParseResponse(SamlInboundContext context)
        {
            var elements = context.Form;
            var responseBase64 = elements[HttpRedirectBindingConstants.SamlResponse];
            var relayState = await this._relayStateHandler.GetRelayStateFromFormData(elements);
            var responseBytes = Convert.FromBase64String(responseBase64);
            var responseText = Encoding.UTF8.GetString(responseBytes);
            this._logProvider.LogMessage(String.Format("Response received:\r\n {0}", responseText));
            var responseTypes = this.GetTypes();
            var type = this._messageTypeResolver.ResolveMessageType(responseText, responseTypes);
            var statusResponse =  this._samlResponseParserFactory(type).Parse(responseText);
            var responseStatus = new SamlResponseContext { StatusResponse = statusResponse, RelayState = relayState, Response = responseText };
            await this._responseValidator.ValidateResponse(responseStatus);
            return responseStatus;
        }

        public IEnumerable<Type> GetTypes()
        {
            var types = ReflectionHelper.GetAllTypes(t => !t.IsAbstract && !t.IsInterface && typeof(StatusResponse).IsAssignableFrom(t));
            return types;
        }
    }
}