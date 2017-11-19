using System;
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
            var url = String.Format("{0}?{1}", base.DestinationUri.AbsoluteUri, base.ClauseBuilder);
            return new Uri(url);
        }
    }
}