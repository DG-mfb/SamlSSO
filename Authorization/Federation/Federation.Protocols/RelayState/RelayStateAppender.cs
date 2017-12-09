using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kernel.Federation.Protocols;
using Kernel.Logging;
using Shared.Federtion.Constants;

namespace Federation.Protocols.RelayState
{
    internal class RelayStateAppender : IRelayStateAppender
    {
        private readonly ILogProvider _logProvider;
        
        public RelayStateAppender(ILogProvider logProvider)
        {
            this._logProvider = logProvider;
        }

        public Task BuildRelayState(AuthnRequestContext authnRequestContext)
        {
            if (authnRequestContext == null)
                throw new ArgumentNullException();
            if (authnRequestContext.RelyingState == null)
                throw new ArgumentNullException("relayiState");

            authnRequestContext.RelyingState[RelayStateContstants.FederationPartyId] = authnRequestContext.FederationPartyContext.FederationPartyId;
            authnRequestContext.RelyingState[RelayStateContstants.RequestId] = authnRequestContext.RequestId;
            authnRequestContext.RelyingState[RelayStateContstants.Origin] = authnRequestContext.Origin;
            this._logProvider.LogMessage(String.Format("Relay state built. Members: {0}", this.FormatMessage(authnRequestContext.RelyingState)));
            
            return Task.CompletedTask;
        }

        private string FormatMessage(IDictionary<string, object> source)
        {
            if (source == null)
                return string.Empty;

            var sb = new StringBuilder();
            return source.Aggregate(sb, (t, next) =>
            {
                t.AppendFormat("Key: {0}, value: {1}\r\n", next.Key, next.Value);
                return t;
            }, r => r.ToString());
        }
    }
}