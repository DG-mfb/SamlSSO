using System;
using System.Collections.Generic;
using System.Text;
using Kernel.Federation.Protocols.Bindings.HttpRedirectBinding;
using Kernel.Federation.Protocols.Request;
using Kernel.Web;

namespace Federation.Protocols.Bindings.HttpRedirect
{
    public class RequestBindingContext : HttpRedirectContext
    {
        public RequestBindingContext(RequestContext requestContext)
            : base(requestContext.RelyingState, requestContext.Destination)
        {
            this.RequestContext = requestContext;
        }
        public RequestContext RequestContext { get; }

        protected override void Format(StringBuilder sb, KeyValuePair<string, string> value)
        {
            sb.AppendFormat("{0}={1}&", value.Key, Uri.EscapeDataString(Utility.UpperCaseUrlEncode(value.Value)));
        }
    }
}