using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kernel.Federation.Protocols.Bindings.HttpRedirectBinding
{
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

        public virtual string BuildQuesryString()
        {
            var clauseBuilder = new StringBuilder();
            var query = base.RequestParts.Aggregate(clauseBuilder, (b, next) =>
            {
                this.Format(b, next);
                return b;
            }, r => r.ToString().TrimEnd('&'));

            return query;
        }

        protected virtual void Format(StringBuilder sb, KeyValuePair<string, string> value)
        {
            sb.AppendFormat("{0}={1}&", value.Key, Uri.EscapeDataString(value.Value));
        }
    }
}