using System;
using System.Collections.Generic;
using Kernel.Federation.Constants;

namespace Kernel.Federation.Protocols
{
    public class SamlInboundMessage
    {
        public SamlInboundMessage(Uri binding, Uri originUrl)
        {
            this.Elements = new Dictionary<string, object>();
            this.Binding = binding;
            this.OriginUrl = originUrl;
        }
        public IDictionary<string, object> Elements { get; }
        public Uri Binding { get; }
        public Uri OriginUrl { get; }
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

        public bool HasRelaySate
        {
            get
            {
                return this.Elements.ContainsKey(HttpRedirectBindingConstants.RelayState);
            }
        }

        public object RelayState
        {
            get
            {
                if (this.HasRelaySate)
                    return this.Elements[HttpRedirectBindingConstants.RelayState];
                return null;
            }
        }

        public bool TryGetRelayState<T>(out T state) where T: class
        {
            var relaySate = this.RelayState;
            state = relaySate as T;
            return state != null;
        }
    }
}