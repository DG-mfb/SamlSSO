using System;
using System.Linq;
using System.Text;
using Kernel.Federation.Protocols;

namespace Federation.Protocols.Bindings.HttpRedirect
{
    public class HttpRedirectContext : RequestBindingContext
    {
        public HttpRedirectContext(AuthnRequestContext authnRequestContext) : base(authnRequestContext)
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
                b.AppendFormat("{0}={1}&", next.Key, next.Value);
                return b;
            }, r => r.ToString().TrimEnd('&'));

            return query;
        }
    }
}