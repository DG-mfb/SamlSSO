using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings;
using Kernel.Logging;

namespace Federation.Protocols.Bindings.HttpPost
{
    internal class PostBindingDecoder : IBindingDecoder<IDictionary<string, string>>
    {
        private readonly ILogProvider _logProvider;
       
        public PostBindingDecoder(ILogProvider logProvider)
        {
            this._logProvider = logProvider;
        }
        public async Task<SamlInboundMessage> Decode(IDictionary<string, string> request)
        {
            var result = new SamlInboundMessage(new Uri(Kernel.Federation.MetaData.Configuration.Bindings.Http_Post), null);
            
            foreach(var el in request)
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
                return new KeyValuePair<string, object>(element.Key, element.Value);
            }
            var elementBytes = Convert.FromBase64String(element.Value);
            var elementText = Encoding.UTF8.GetString(elementBytes);
            this._logProvider.LogMessage(String.Format("Element: {0} decoded:\r\n {1}",element.Key, elementText));
            return new KeyValuePair<string, object>(element.Key, elementText);
        }
    }
}