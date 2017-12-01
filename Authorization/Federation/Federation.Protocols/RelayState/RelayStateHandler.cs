using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kernel.Configuration;
using Kernel.Federation.Protocols;
using Kernel.Logging;

namespace Federation.Protocols.RelayState
{
    internal class RelayStateHandler : IRelayStateHandler
    {
        private readonly IRelayStateSerialiser _relayStateSerialiser;
        private readonly ILogProvider _logProvider;
        public ICustomConfigurator<IDictionary<string, object>> RelayStateCustomConfuguration { private get; set; }
        public RelayStateHandler(IRelayStateSerialiser relayStateSerialiser, ILogProvider logProvider)
        {
            this._logProvider = logProvider;
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
            authnRequestContext.RelyingState["requestId"] = authnRequestContext.RequestId;
            authnRequestContext.RelyingState["origin"] = authnRequestContext.Origin;
            this._logProvider.LogMessage(String.Format("Relay state built. Members: {0}", this.FormatMessage(authnRequestContext.RelyingState)));
            if (this.RelayStateCustomConfuguration != null)
            {
                this.RelayStateCustomConfuguration.Configure(authnRequestContext.RelyingState);
                this._logProvider.LogMessage(String.Format("Relay state customised. Members: {0}", this.FormatMessage(authnRequestContext.RelyingState)));
            }
            return Task.CompletedTask;
        }

        public async Task<object> GetRelayStateFromFormData(IDictionary<string, string> form)
        {
            var relayStateCompressed = form["RelayState"];
            var relayState = await this._relayStateSerialiser.Deserialize(relayStateCompressed);
            return relayState;
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