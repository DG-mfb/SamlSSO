using System;
using System.Text;
using System.Threading.Tasks;
using Federation.Protocols.Response.Validation;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Response;
using Kernel.Logging;
using Shared.Federtion.Constants;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class ResponseParser : IResponseParser<SamlInboundContext, SamlResponseContext>
    {
        private readonly ILogProvider _logProvider;
        private readonly ResponseValidator _responseValidator;
        private readonly SamlTokenResponseParser _samlResponseParser;
        private readonly IRelayStateHandler _relayStateHandler;
        public ResponseParser(ILogProvider logProvider, IRelayStateHandler relayStateHandler, ResponseValidator responseValidator)
        {
            this._relayStateHandler = relayStateHandler;
            this._logProvider = logProvider;
            this._responseValidator = responseValidator;
            this._samlResponseParser = new SamlTokenResponseParser(logProvider);
        }
        public async Task<SamlResponseContext> ParseResponse(SamlInboundContext context)
        {
            var elements = context.Form;
            var responseBase64 = elements[HttpRedirectBindingConstants.SamlResponse];
            var relayState = await this._relayStateHandler.GetRelayStateFromFormData(elements);
            var responseBytes = Convert.FromBase64String(responseBase64);
            var responseText = Encoding.UTF8.GetString(responseBytes);
            this._logProvider.LogMessage(String.Format("Response received:\r\n {0}", responseText));
            var statusResponse = await this._samlResponseParser.ParseResponse(responseText);
            var responseStatus = new SamlResponseContext { StatusResponse = statusResponse, RelayState = relayState, Response = responseText };
            await this._responseValidator.ValidateResponse(responseStatus, elements);
            return responseStatus;
        }
    }
}