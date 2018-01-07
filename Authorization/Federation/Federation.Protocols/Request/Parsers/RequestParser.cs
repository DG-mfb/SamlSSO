using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Federation.Protocols.Request.Validation;
using Federation.Protocols.Response.Validation;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;
using Kernel.Logging;
using Kernel.Reflection;
using Shared.Federtion.Models;
using Shared.Federtion.Request;
using Shared.Federtion.Response;

namespace Federation.Protocols.Request.Parsers
{
    internal class RequestParser : IMessageParser<SamlInboundContext, SamlInboundRequestContext>
    {
        private readonly ILogProvider _logProvider;
        private readonly RequestValidator _requestValidator;
        private readonly Func<Type, SamlRequestParser> _samlResponseParserFactory;
        private readonly MessageTypeResolver _messageTypeResolver = new MessageTypeResolver();
        private readonly IRelayStateHandler _relayStateHandler;
        public RequestParser(Func<Type, SamlRequestParser> samlResponseParserFactory, IRelayStateHandler relayStateHandler, ILogProvider logProvider, RequestValidator requestValidator)
        {
            this._samlResponseParserFactory = samlResponseParserFactory;
            this._logProvider = logProvider;
            this._requestValidator = requestValidator;
            this._relayStateHandler = relayStateHandler;
        }
        public async Task<SamlInboundRequestContext> Parse(SamlInboundContext context)
        {
            var message = context.Message;
            var requestText = message.Elements[HttpRedirectBindingConstants.SamlRequest].ToString();
            
            var responseTypes = this.GetTypes();
            var type = this._messageTypeResolver.ResolveMessageType(requestText, responseTypes);
            var reqeuest =  this._samlResponseParserFactory(type).Parse(requestText);
            
            var requestContext = new SamlInboundRequestContext { SamlRequest = reqeuest, SamlInboundMessage = message, Request = requestText };
            //await this._requestValidator.ValidateIRequest(requestContext);
            return requestContext;
        }

        public IEnumerable<Type> GetTypes()
        {
            var types = ReflectionHelper.GetAllTypes(t => !t.IsAbstract && !t.IsInterface && typeof(RequestAbstract).IsAssignableFrom(t));
            return types;
        }
    }
}