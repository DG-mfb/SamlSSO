using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Kernel.Extensions;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using Kernel.Federation.Protocols.Response;
using Kernel.Federation.Tokens;
using Kernel.Logging;
using Shared.Federtion.Constants;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class ResponseHandler : IReponseHandler<ClaimsIdentity>
    {
        private readonly ITokenHandler _tokenHandler;
        private readonly ILogProvider _logProvider;
        private readonly IResponseParser<HttpPostResponseContext, ResponseStatus> _responseParser;

        public ResponseHandler(IResponseParser<HttpPostResponseContext, ResponseStatus> responseParser, ITokenHandler tokenHandler, ILogProvider logProvider)
        {
            this._responseParser = responseParser;
            this._tokenHandler = tokenHandler;
            this._logProvider = logProvider;
        }
        public async Task<ClaimsIdentity> Handle(HttpPostResponseContext context)
        {
            try
            {
                var responseStatus = await this._responseParser.ParseResponse(context);
                using (var reader = new StringReader(responseStatus.Response))
                {
                    using (var xmlReader = XmlReader.Create(reader))
                    {
                        var handlerContext = new HandleTokenContext(xmlReader, responseStatus.RelayState, context.AuthenticationMethod);
                        var response = await this._tokenHandler.HandleToken(handlerContext);
                        if (!response.IsValid)
                            throw new Exception(EnumerableExtensions.Aggregate(response.ValidationResults.Select(x => x.ErrorMessage)));
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