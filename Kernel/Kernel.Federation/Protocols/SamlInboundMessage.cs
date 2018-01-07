using System;
using System.Collections.Generic;
using Kernel.Federation.Constants;

namespace Kernel.Federation.Protocols
{
    public class SamlInboundMessage
    {
        public SamlInboundMessage(Uri binding, Uri requestUrl)
        {
            this.Elements = new Dictionary<string, object>();
            this.Binding = binding;
            this.RequestUrl = requestUrl;
        }
        public IDictionary<string, object> Elements { get; }
        public Uri Binding { get; }
        public Uri RequestUrl { get; }
        public bool IsSamlRequest
        {
            get
            {
                return this.Elements.ContainsKey(HttpRedirectBindingConstants.SamlRequest);
            }
        }
        public bool IsSamlRsponse
        {
            get
            {
                return this.Elements.ContainsKey(HttpRedirectBindingConstants.SamlResponse);
            }
        }
    }
}