using System;
using System.Linq;
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
       
        private readonly IMessageEncoding _messageEncoding;
        
        public RedirectBindingDecoder(ILogProvider logProvider, IMessageEncoding messageEncoding)
        {
            this._messageEncoding = messageEncoding;
            this._logProvider = logProvider;
        }
        public async Task<SamlInboundMessage> Decode(Uri request)
        {
            var source = request.Query.TrimStart('?').Split('&')
                .Select(x => x.Split('='))
                .ToDictionary(k => k[0], v => v[1]);
                
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
                return new KeyValuePair<string, object>(element.Key, Uri.UnescapeDataString(element.Value));
            }
            if (element.Key == HttpRedirectBindingConstants.SamlRequest || element.Key == HttpRedirectBindingConstants.SamlResponse)
            {
                var value = await this._messageEncoding.DecodeMessage(Uri.UnescapeDataString(element.Value));
                return new KeyValuePair<string, object>(element.Key, value);
            }
            var elementText = Uri.UnescapeDataString(element.Value);
            this._logProvider.LogMessage(String.Format("Element: {0} decoded:\r\n {1}", element.Key, elementText));
            return new KeyValuePair<string, object>(element.Key, elementText);
        }
    }
}