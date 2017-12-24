using System;
using System.Text;
using Kernel.Federation.Protocols;

namespace Federation.Protocols.Bindings.HttpRedirect
{
    public class HttpRedirectContext : RequestBindingContext
    {
        public HttpRedirectContext(AuthnRequestContext authnRequestContext) : base(authnRequestContext)
        {
            this.ClauseBuilder = new StringBuilder();
        }
        public StringBuilder ClauseBuilder { get; }
        public override Uri GetDestinationUrl()
        {
            var url = String.Format("{0}?{1}", base.DestinationUri.AbsoluteUri, this.ClauseBuilder);
            return new Uri(url);
        }
    }
}