using System;
using System.Collections.Generic;
using Kernel.Federation.Constants;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class ResponseHelper
    {
        internal static void EnsureSuccessAndThrow(SamlInboundResponseContext status)
        {
            if (status.IsSuccess)
                return;
            var msg = status.AggregatedMessage;
            throw new Exception(msg);
        }

        internal static void EnsureRequestIdMatch(object state, string requestId)
        {
            var relayState = state as IDictionary<string, object>;
            if (relayState == null)
                throw new ArgumentNullException("relay state");
            var requestIdFromState = relayState[RelayStateContstants.RequestId].ToString();
            if (String.Equals(requestId, requestIdFromState, StringComparison.Ordinal))
                return;
            throw new InvalidOperationException("RequestId and InResponseTo don't match.");
        }
    }
}