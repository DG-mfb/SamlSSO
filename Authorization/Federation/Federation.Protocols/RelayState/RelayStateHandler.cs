using System.Collections.Generic;
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

        public async Task<object> GetRelayStateFromFormData(IDictionary<string, string> form)
        {
            var relayStateCompressed = form["RelayState"];
            var relayState = await this._relayStateSerialiser.Deserialize(relayStateCompressed);
            return relayState;
        }
    }
}