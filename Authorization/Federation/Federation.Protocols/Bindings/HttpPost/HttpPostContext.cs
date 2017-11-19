using Kernel.Federation.Protocols;

namespace Federation.Protocols.Bindings.HttpPost
{
    public class HttpPostContext : BindingContext
    {
        public HttpPostContext(AuthnRequestContext authnRequestContext) : base(authnRequestContext.RelyingState, authnRequestContext.Destination)
        {
            this.AuthnRequestContext = authnRequestContext;
        }
    }
}