using System;
using System.Linq;
using System.Threading.Tasks;
using Kernel.Extensions;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using Kernel.Federation.Tokens;
using Kernel.Logging;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class ResponseHandler : IInboundHandler<HttpPostResponseInboundContext>
    {
        private readonly ITokenHandler _tokenHandler;
        private readonly ILogProvider _logProvider;
        private readonly IMessageParser<SamlInboundContext, SamlInboundResponseContext> _responseParser;

        public ResponseHandler(IMessageParser<SamlInboundContext, SamlInboundResponseContext> responseParser, ITokenHandler tokenHandler, ILogProvider logProvider)
        {
            this._responseParser = responseParser;
            this._tokenHandler = tokenHandler;
            this._logProvider = logProvider;
        }
        public async Task Handle(HttpPostResponseInboundContext context)
        {
            try
            {
                if (context == null)
                    throw new ArgumentNullException("context");
               
                var responseStatus = await this._responseParser.Parse(context);
                
                var samlResponse = responseStatus.StatusResponse as TokenResponse;
               
                var hasToken = (samlResponse != null && samlResponse.Assertions != null && samlResponse.Assertions.Length == 1);
                context.HasSecurityToken = hasToken;
                this._logProvider.LogMessage(String.Format("Response is{0} a security token carrier.", hasToken ? String.Empty : " not"));
                if (hasToken)
                {
                    var token = samlResponse.Assertions[0];
                    var handlerContext = new HandleTokenContext(token, responseStatus.FederationPartyId, context.AuthenticationMethod, responseStatus.SamlInboundMessage.RelayState);
                    var response = await this._tokenHandler.HandleToken(handlerContext);
                    if (!response.IsValid)
                        throw new Exception(EnumerableExtensions.Aggregate(response.ValidationResults.Select(x => x.ErrorMessage)));
                    context.Identity = response.Identity;
                }
            }
            catch (Exception ex)
            {
                Exception innerEx;
                this._logProvider.TryLogException(ex, out innerEx);
                if (innerEx != null)
                    throw innerEx;
            }
        }
    }
}