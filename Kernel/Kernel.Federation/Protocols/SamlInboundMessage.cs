using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using Kernel.Cryptography;
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

        public string SamlMessage
        {
            get
            {
                if (this.IsSamlRequest)
                    return this.Elements[HttpRedirectBindingConstants.SamlRequest].ToString();
                if (this.IsSamlRsponse)
                    return this.Elements[HttpRedirectBindingConstants.SamlResponse].ToString();
                throw new InvalidOperationException("No saml massage element");
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

        public bool IsSigned
        {
            get
            {
                return this.Elements.ContainsKey(HttpRedirectBindingConstants.Signature);
            }
        }

        public DataSignatureDescriptor Signature
        {
            get
            {
                if (!this.IsSigned)
                    throw new InvalidOperationException("No signature element found.");
                var signature = this.Elements[HttpRedirectBindingConstants.Signature].ToString();
                var sigAlg = SignedXml.XmlDsigRSASHA1Url;
                if (this.Elements.ContainsKey(HttpRedirectBindingConstants.SigAlg))
                    sigAlg = this.Elements[HttpRedirectBindingConstants.SigAlg].ToString();
                return new DataSignatureDescriptor(sigAlg, signature);
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