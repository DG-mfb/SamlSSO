using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Federation.Protocols.Response.Validation;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Response;
using Kernel.Logging;
using Kernel.Reflection;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class ResponseParser : IResponseParser<SamlInboundContext, SamlResponseContext>
    {
        private readonly ILogProvider _logProvider;
        private readonly ResponseValidator _responseValidator;
        private readonly Func<Type, SamlResponseParser> _samlResponseParserFactory;
        private readonly MessageTypeResolver _messageTypeResolver = new MessageTypeResolver();
        public ResponseParser(Func<Type, SamlResponseParser> samlResponseParserFactory, ILogProvider logProvider, ResponseValidator responseValidator)
        {
            this._samlResponseParserFactory = samlResponseParserFactory;
            this._logProvider = logProvider;
            this._responseValidator = responseValidator;
        }
        public async Task<SamlResponseContext> ParseResponse(SamlInboundContext context)
        {
            var message = context.Message;
            var responseText = message.Elements[HttpRedirectBindingConstants.SamlResponse].ToString();
            var relayState = message.Elements[HttpRedirectBindingConstants.RelayState];
            var responseTypes = this.GetTypes();
            var type = this._messageTypeResolver.ResolveMessageType(responseText, responseTypes);
            var statusResponse =  this._samlResponseParserFactory(type).Parse(responseText);
            var responseContext = new SamlResponseContext { StatusResponse = statusResponse, RelayState = relayState, Response = responseText };
            await this._responseValidator.ValidateResponse(responseContext);
            return responseContext;
        }

        public IEnumerable<Type> GetTypes()
        {
            var types = ReflectionHelper.GetAllTypes(t => !t.IsAbstract && !t.IsInterface && typeof(StatusResponse).IsAssignableFrom(t));
            return types;
        }
    }
}