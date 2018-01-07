using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings;
using Kernel.Logging;

namespace Federation.Protocols.Bindings.HttpRedirect
{
    internal class RedirectBindingDecoder : IBindingDecoder<Uri>
    {
        private readonly ILogProvider _logProvider;
       
        private readonly IRelayStateHandler _relayStateHandler;
        private readonly IMessageEncoding _messageEncoding;
        private readonly Func<Uri, IDictionary<string, string>> _formater;
        
        public RedirectBindingDecoder(ILogProvider logProvider, IRelayStateHandler relayStateHandler, IMessageEncoding messageEncoding, Func<Uri, IDictionary<string, string>> formater)
        {
            this._relayStateHandler = relayStateHandler;
            this._messageEncoding = messageEncoding;
            this._logProvider = logProvider;
            this._formater = formater;
        }
        public async Task<SamlInboundMessage> Decode(Uri request)
        {
            var source = this._formater(request);
            var result = new SamlInboundMessage(new Uri(Kernel.Federation.MetaData.Configuration.Bindings.Http_Redirect), request);
            
            foreach (var el in source)
            {
                var decoded = await this.DecodeElement(el);
                result.Elements.Add(decoded.Key, decoded.Value);
            }
            return result;
        }

        public async Task<KeyValuePair<string, object>> DecodeElement(KeyValuePair<string, string> element)
        {
            if(element.Key == HttpRedirectBindingConstants.RelayState)
            {
                var value = await this._relayStateHandler.Decode(element.Value);
                return new KeyValuePair<string, object>(element.Key, value);
            }
            if (element.Key == HttpRedirectBindingConstants.SamlRequest || element.Key == HttpRedirectBindingConstants.SamlResponse)
            {
                var value = await this._messageEncoding.DecodeMessage(element.Value);
                return new KeyValuePair<string, object>(element.Key, value);
            }
            var elementText = Uri.UnescapeDataString(element.Value);
            this._logProvider.LogMessage(String.Format("Element: {0} decoded:\r\n {1}", element.Key, elementText));
            return new KeyValuePair<string, object>(element.Key, elementText);
        }
    }
}