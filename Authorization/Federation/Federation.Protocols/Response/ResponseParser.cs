using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Federation.Protocols.Response.Validation;
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
        private readonly ILogProvider _logProvider;

        public ResponseParser(ILogProvider logProvider)
        {
            this._logProvider = logProvider;
        }
        public async Task<ResponseStatus> ParseResponse(HttpPostResponseContext context)
        {
            var elements = context.Form;
            var responseBase64 = elements[HttpRedirectBindingConstants.SamlResponse];
            var responseBytes = Convert.FromBase64String(responseBase64);
            var responseText = Encoding.UTF8.GetString(responseBytes);
            this._logProvider.LogMessage(String.Format("Response received:\r\n {0}", responseText));
            var responseStatus = ResponseHelper.ParseResponseStatus(responseText, this._logProvider);
            var service = ApplicationConfiguration.Instance.DependencyResolver.Resolve<ResponseValidator>();
            await service.ValidateResponse(responseStatus, elements);

            //if (!responseStatus.IsIdpInitiated)
            //{
            //    var service = ApplicationConfiguration.Instance.DependencyResolver.Resolve<ResponseValidator>();
            //    await service.ValidateResponse(responseStatus, elements);
            //}
            //else
            //{
            //    var service = ApplicationConfiguration.Instance.DependencyResolver.Resolve<IdpInitDiscoveryService>();
            //    var federationParnerId = service.ResolveParnerId(responseStatus);
            //    if (String.IsNullOrWhiteSpace(federationParnerId))
            //        throw new InvalidOperationException(String.Format("Unsolicited Web SSO initiated by unknow issuer. Issuer: {0}", responseStatus.Issuer));

            //    responseStatus.RelayState = new Dictionary<string, object> { { RelayStateContstants.FederationPartyId, federationParnerId } };
            //}
            return responseStatus;
        }
    }
}