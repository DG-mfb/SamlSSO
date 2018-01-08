using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kernel.Federation.Constants;
using Kernel.Logging;

namespace Federation.Protocols.Response.Validation.ValidationRules
{
    internal class IssuerKnownRule : ResponseValidationRule
    {
        private readonly IdpInitDiscoveryService _service;

        public IssuerKnownRule(IdpInitDiscoveryService service, ILogProvider logProvider)
            : base(logProvider)
        {
            this._service = service;
        }

        internal override RuleScope Scope
        {
            get
            {
                return RuleScope.IdpInitiated;
            }
        }

        protected override Task<bool> ValidateInternal(SamlResponseValidationContext context)
        {
            base._logProvider.LogMessage("Issuer Known Rule running.");
            var federationParnerId = this._service.ResolveParnerId(context.ResponseContext);
            if (String.IsNullOrWhiteSpace(federationParnerId))
                throw new InvalidOperationException(String.Format("Unsolicited Web SSO initiated by unknow issuer. Issuer: {0}", context.ResponseContext.StatusResponse.Issuer.Value));

            context.ResponseContext.SamlInboundMessage.Elements[HttpRedirectBindingConstants.RelayState] = new Dictionary<string, object> { { RelayStateContstants.FederationPartyId, federationParnerId } };

            return Task.FromResult(true);
        }
    }
}