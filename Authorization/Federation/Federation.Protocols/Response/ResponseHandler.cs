using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml;
using Kernel.Extensions;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using Kernel.Federation.Protocols.Response;
using Kernel.Federation.Tokens;
using Kernel.Logging;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class ResponseHandler : IInboundHandler<ClaimsIdentity>
    {
        private readonly ITokenHandler _tokenHandler;
        private readonly ILogProvider _logProvider;
        private readonly IResponseParser<HttpPostInboundContext, ResponseStatus> _responseParser;

        public ResponseHandler(IResponseParser<HttpPostInboundContext, ResponseStatus> responseParser, ITokenHandler tokenHandler, ILogProvider logProvider)
        {
            this._responseParser = responseParser;
            this._tokenHandler = tokenHandler;
            this._logProvider = logProvider;
        }
        public async Task<ClaimsIdentity> Handle(SamlInboundContext context)
        {
            try
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                var httpPostContext = context as HttpPostInboundContext;
                if (httpPostContext == null)
                    throw new InvalidOperationException(String.Format("Expected context of type: {0} but it was: {1}", typeof(HttpPostInboundContext).Name, context.GetType().Name));

                var responseStatus = await this._responseParser.ParseResponse(httpPostContext);
                context.RelayState = responseStatus.RelayState;
                using (var reader = new StringReader(responseStatus.Response))
                {
                    using (var xmlReader = XmlReader.Create(reader))
                    {
                        var handlerContext = new HandleTokenContext(xmlReader, responseStatus.FederationPartyId, httpPostContext.AuthenticationMethod, responseStatus.RelayState);
                        var response = await this._tokenHandler.HandleToken(handlerContext);
                        if (!response.IsValid)
                            throw new Exception(EnumerableExtensions.Aggregate(response.ValidationResults.Select(x => x.ErrorMessage)));
                        httpPostContext.Identity = response.Identity;
                        return response.Identity;
                    }
                }
            }
            catch(Exception ex)
            {
                Exception innerEx;
                this._logProvider.TryLogException(ex, out innerEx);
                if(innerEx != null)
                    throw innerEx;
            }
            return null;
        }
    }
}