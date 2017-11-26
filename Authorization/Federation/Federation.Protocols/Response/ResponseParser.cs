using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using Kernel.Federation.Protocols.Response;
using Kernel.Logging;
using Shared.Federtion.Constants;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class ResponseParser : IResponseParser<HttpPostResponseContext, ResponseStatus>
    {
        private readonly IRelayStateHandler _relayStateHandler;
        private readonly ILogProvider _logProvider;

        public ResponseParser(IRelayStateHandler relayStateHandler, ILogProvider logProvider)
        {
            this._relayStateHandler = relayStateHandler;
            this._logProvider = logProvider;

        }
        public async Task<ResponseStatus> ParseResponse(HttpPostResponseContext context)
        {
            var elements = context.Form;
            var responseBase64 = elements[HttpRedirectBindingConstants.SamlResponse];
            var responseBytes = Convert.FromBase64String(responseBase64);
            var responseText = Encoding.UTF8.GetString(responseBytes);
            this._logProvider.LogMessage(String.Format("Response recieved:\r\n {0}", responseText));
            var responseStatus = ResponseHelper.ParseResponseStatus(responseText, this._logProvider);
            ResponseHelper.EnsureSuccessAndThrow(responseStatus);
            
            var relayState = await this._relayStateHandler.GetRelayStateFromFormData(elements);
            responseStatus.RelayState = relayState;
            ResponseHelper.EnsureRequestPathMatch(relayState, context.RequestUri);
            return responseStatus;
        }
    }
}
