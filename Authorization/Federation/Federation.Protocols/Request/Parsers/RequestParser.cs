using Federation.Protocols.Request.Validation;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.Protocols;
using Kernel.Logging;
using Kernel.Reflection;
using Shared.Federtion.Factories;
using Shared.Federtion.Models;
using Shared.Federtion.Request;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Federation.Protocols.Request.Parsers
{
    internal class RequestParser : IMessageParser<SamlInboundContext, SamlInboundRequestContext>
    {
        private readonly ILogProvider _logProvider;
        private readonly RequestValidator _requestValidator;
        private readonly Func<Type, SamlRequestParser> _samlResponseParserFactory;
        private readonly MessageTypeResolver _messageTypeResolver = new MessageTypeResolver();
        private readonly IConfigurationManager<MetadataBase> _configurationManager;
        private readonly Func<Type, IMetadataHandler> _metadataHandlerFactory;
        public RequestParser(Func<Type, IMetadataHandler> metadataHandlerFactory, Func<Type, SamlRequestParser> samlResponseParserFactory, IConfigurationManager<MetadataBase> configurationManager, ILogProvider logProvider, RequestValidator requestValidator)
        {
            this._metadataHandlerFactory = metadataHandlerFactory;
            this._samlResponseParserFactory = samlResponseParserFactory;
            this._logProvider = logProvider;
            this._requestValidator = requestValidator;
            this._configurationManager = configurationManager;
        }
        public async Task<SamlInboundRequestContext> Parse(SamlInboundContext context)
        {
            var message = context.Message;
            var requestText = message.SamlMessage;
            
            var responseTypes = this.GetTypes();
            var type = this._messageTypeResolver.ResolveMessageType(requestText, responseTypes);
            var request =  this._samlResponseParserFactory(type).Parse(requestText);
            
            var requestContext = new SamlInboundRequestContext { SamlRequest = request, SamlInboundMessage = message, OriginUrl = message.OriginUrl };
            await this.ResolveIssuerKeys(requestContext, context.DescriptorResolver);
            await this._requestValidator.ValidateIRequest(requestContext);
            return requestContext;
        }

        private async Task ResolveIssuerKeys(SamlInboundRequestContext context, Func<MetadataBase, RoleDescriptor> resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException("roleDescriptor resolver");
            var configuration = await this._configurationManager.GetConfigurationAsync(context.SamlRequest.Issuer.Value, CancellationToken.None);
            var descriptor = resolver(configuration);
            var keys = descriptor.Keys.Where(x => x.Use == KeyType.Signing);
            keys.Aggregate(context.Keys, (t, next) => { t.Add(next); return t; });
        }

        private IEnumerable<Type> GetTypes()
        {
            var types = ReflectionHelper.GetAllTypes(t => !t.IsAbstract && !t.IsInterface && typeof(RequestAbstract).IsAssignableFrom(t));
            return types;
        }
    }
}