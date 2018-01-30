using Federation.Protocols.Response.Validation;
using Kernel.Federation.Constants;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.Protocols;
using Kernel.Logging;
using Kernel.Reflection;
using Shared.Federtion.Factories;
using Shared.Federtion.Response;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Federation.Protocols.Response
{
    internal class ResponseParser : IMessageParser<SamlInboundContext, SamlInboundResponseContext>
    {
        private readonly ILogProvider _logProvider;
        private readonly ResponseValidator _responseValidator;
        private readonly Func<Type, SamlResponseParser> _samlResponseParserFactory;
        private readonly MessageTypeResolver _messageTypeResolver = new MessageTypeResolver();
        private readonly IRelayStateHandler _relayStateHandler;
        private readonly IConfigurationManager<MetadataBase> _configurationManager;
        private readonly Func<Type, IMetadataHandler> _metadataHandlerFactory;
        public ResponseParser(Func<Type, IMetadataHandler> metadataHandlerFactory, Func<Type, SamlResponseParser> samlResponseParserFactory, IConfigurationManager<MetadataBase> configurationManager, IRelayStateHandler relayStateHandler, ILogProvider logProvider, ResponseValidator responseValidator)
        {
            this._metadataHandlerFactory = metadataHandlerFactory;
            this._configurationManager = configurationManager;
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
            await this.ResolveIssuerKeys(responseContext, context.DescriptorResolver);
            await this._responseValidator.ValidateResponse(responseContext);
            return responseContext;
        }

        public IEnumerable<Type> GetTypes()
        {
            var types = ReflectionHelper.GetAllTypes(t => !t.IsAbstract && !t.IsInterface && typeof(StatusResponse).IsAssignableFrom(t));
            return types;
        }

        private async Task ResolveIssuerKeys(SamlInboundResponseContext context, Func<MetadataBase, RoleDescriptor> resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException("roleDescriptor resolver");
            var configuration = await this._configurationManager.GetConfigurationAsync(context.FederationPartyId, CancellationToken.None);
            var descriptor = resolver(configuration);
            var keys = descriptor.Keys.Where(x => x.Use == KeyType.Signing);
            keys.Aggregate(context.Keys, (t, next) => { t.Add(next); return t; });
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