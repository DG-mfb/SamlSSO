using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kernel.Federation.Protocols;
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
    }
    public class HttpRedirectContext : BindingContext
    {
        public HttpRedirectContext(IDictionary<string, object> relayState, Uri destinationUri) : base(relayState, destinationUri)
        {
        }

        public override Uri GetDestinationUrl()
        {
            var query = this.BuildQuesryString();
            var url = String.Format("{0}?{1}", base.DestinationUri.AbsoluteUri, query);
            return new Uri(url);
        }

        internal string BuildQuesryString()
        {
            var clauseBuilder = new StringBuilder();
            var query = base.RequestParts.Aggregate(clauseBuilder, (b, next) =>
            {
                b.AppendFormat("{0}={1}&", next.Key, Uri.EscapeDataString(Utility.UpperCaseUrlEncode(next.Value)));
                return b;
            }, r => r.ToString().TrimEnd('&'));

            return query;
        }
    }
}