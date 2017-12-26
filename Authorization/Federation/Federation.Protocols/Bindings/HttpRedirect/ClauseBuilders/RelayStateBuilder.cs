using System;
using System.Threading.Tasks;
using Kernel.Federation.Protocols;
using Shared.Federtion.Constants;

namespace Federation.Protocols.Bindings.HttpRedirect.ClauseBuilders
{
    internal class RelayStateBuilder : IRedirectClauseBuilder
    {
        private readonly IRelayStateSerialiser _relayStateSerialiser;
        public RelayStateBuilder(IRelayStateSerialiser relayStateSerialiser)
        {
            this._relayStateSerialiser = relayStateSerialiser;
        }
        public uint Order { get { return 1; } }

        public async Task Build(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (context.RelayState == null || context.RelayState.Count == 0)
                return;

            var httpRedirectContext = context as HttpRedirectContext;
            if (httpRedirectContext == null)
                throw new InvalidOperationException(String.Format("Binding context must be of type:{0}. It was: {1}", typeof(HttpRedirectContext).Name, context.GetType().Name));

            var rsEncoded = await this._relayStateSerialiser.Serialize(context.RelayState);
            if (String.IsNullOrWhiteSpace(rsEncoded))
                throw new InvalidOperationException("Invalid relay state after serialisation. It was null or empty string.");

            var rsEncodedEscaped = Uri.EscapeDataString(Helper.UpperCaseUrlEncode(rsEncoded));
            context.RequestParts.Add(HttpRedirectBindingConstants.RelayState, rsEncodedEscaped);
        }
    }
}