﻿using System;
using System.Threading.Tasks;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;

namespace Federation.Protocols.Bindings.HttpRedirect.ClauseBuilders
{
    internal class RelayStateBuilder : IRedirectClauseBuilder
    {
        private readonly IRelayStateSerialiser _relayStateSerialiser;
        public RelayStateBuilder(IRelayStateSerialiser relayStateSerialiser)
        {
            this._relayStateSerialiser = relayStateSerialiser;
        }
        public uint Order { get { return 2; } }

        public async Task Build(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (context.RelayState == null || context.RelayState.Count == 0)
                return;

            var rsEncoded = await this._relayStateSerialiser.Serialize(context.RelayState);
            if (String.IsNullOrWhiteSpace(rsEncoded))
                throw new InvalidOperationException("Invalid relay state after serialisation. It was null or empty string.");

            context.RequestParts.Add(HttpRedirectBindingConstants.RelayState, rsEncoded);
        }
    }
}