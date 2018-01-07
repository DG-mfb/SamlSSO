using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings;
using Kernel.Logging;
using Shared.Federtion.Constants;

namespace Federation.Protocols.Bindings.HttpPost
{
    internal class PostBindingDecoder : IBindingDecoder<IDictionary<string, string>>
    {
        private readonly ILogProvider _logProvider;
       
        private readonly IRelayStateHandler _relayStateHandler;
        
        public PostBindingDecoder(ILogProvider logProvider, IRelayStateHandler relayStateHandler)
        {
            this._relayStateHandler = relayStateHandler;
            this._logProvider = logProvider;
        }
        public async Task<IDictionary<string, object>> Decode(IDictionary<string, string> request)
        {
            var result = new Dictionary<string, object>();
            foreach(var el in request)
            {
                var decoded = await this.DecodeElement(el);
                result.Add(decoded.Key, decoded.Value);
            }
            return result;
        }

        private async Task<KeyValuePair<string, object>> DecodeElement(KeyValuePair<string, string> element)
        {
            if(element.Key == HttpRedirectBindingConstants.RelayState)
            {
                var value = await this._relayStateHandler.Decode(element.Value);
                return new KeyValuePair<string, object>(element.Key, value);
            }
            var elementBytes = Convert.FromBase64String(element.Value);
            var elementText = Encoding.UTF8.GetString(elementBytes);
            this._logProvider.LogMessage(String.Format("Element: {0} decoded:\r\n {1}",element.Key, elementText));
            return new KeyValuePair<string, object>(element.Key, elementText);
        }
    }
}