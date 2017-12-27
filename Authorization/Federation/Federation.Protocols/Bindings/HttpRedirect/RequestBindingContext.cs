using System;
using System.Collections.Generic;
using System.Text;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpRedirectBinding;
using Kernel.Web;

namespace Federation.Protocols.Bindings.HttpRedirect
{
    public class RequestBindingContext : HttpRedirectContext
    {
        public RequestBindingContext(AuthnRequestContext authnRequestContext)
            : base(authnRequestContext.RelyingState, authnRequestContext.Destination)
        {
            this.AuthnRequestContext = authnRequestContext;
        }
        public AuthnRequestContext AuthnRequestContext { get; }

        protected override void Format(StringBuilder sb, KeyValuePair<string, string> value)
        {
            sb.AppendFormat("{0}={1}&", value.Key, Uri.EscapeDataString(Utility.UpperCaseUrlEncode(value.Value)));
        }
    }
}