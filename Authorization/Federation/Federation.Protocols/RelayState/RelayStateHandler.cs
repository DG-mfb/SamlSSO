﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kernel.Federation.Protocols;

namespace Federation.Protocols.RelayState
{
    internal class RelayStateHandler : IRelayStateHandler
    {
        private readonly IRelayStateSerialiser _relayStateSerialiser;
        public RelayStateHandler(IRelayStateSerialiser relayStateSerialiser)
        {
            this._relayStateSerialiser = relayStateSerialiser;
        }

        public Task BuildRelayState(AuthnRequestContext authnRequestContext)
        {
            if (authnRequestContext == null)
                throw new ArgumentNullException();
            if (authnRequestContext.RelyingState == null)
                throw new ArgumentNullException("relayiState");

            authnRequestContext.RelyingState["federationPartyId"] = authnRequestContext.FederationPartyContext.FederationPartyId;
            authnRequestContext.RelyingState["assertionConsumerServices"] = authnRequestContext.FederationPartyContext.MetadataContext.EntityDesriptorConfiguration.SPSSODescriptors.First().AssertionConsumerServices;
            return Task.CompletedTask;
        }

        public async Task<object> GetRelayStateFromFormData(IDictionary<string, string> form)
        {
            var relayStateCompressed = form["RelayState"];
            var relayState = await this._relayStateSerialiser.Deserialize(relayStateCompressed);
            return relayState;
        }
    }
}