using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using Kernel.Federation.Protocols.Response;
using Kernel.Initialisation;
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
            if (!responseStatus.IsIdpInitiated)
            {
                var relayState = await this._relayStateHandler.GetRelayStateFromFormData(elements);
                if (relayState == null)
                    throw new InvalidOperationException("Relay state is missing in the response.");

                responseStatus.RelayState = relayState;

                var relayStateDictionary = relayState as IDictionary<string, object>;
                if (relayStateDictionary == null)
                    throw new InvalidOperationException(String.Format("Expected relay state type of: {0}, but it was: {1}", typeof(IDictionary<string, object>).Name, relayState.GetType().Name));
                var partnerId = relayStateDictionary[RelayStateContstants.FederationPartyId];

                responseStatus.FederationPartyId = partnerId.ToString();

                ResponseHelper.EnsureRequestIdMatch(relayState, responseStatus.InResponseTo);
            }
            else
            {
                throw new NotSupportedException("Idp initiated SSO is not supported.");
                var service = ApplicationConfiguration.Instance.DependencyResolver.Resolve<IdpInitDiscoveryService>();
                var federationParnerId = service.ResolveParnerId(responseStatus);
                
                responseStatus.RelayState = new Dictionary<string, object> { { RelayStateContstants.FederationPartyId, federationParnerId} };
                responseStatus.FederationPartyId = federationParnerId;
            }
            return responseStatus;
        }
    }
}