using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using Shared.Federtion.Forms;

namespace Federation.Protocols.Bindings.HttpPost
{
    public class RequestPostBindingContext : HttpPostContext
    {
        public RequestPostBindingContext(RequestContext authnRequestContext)
            : base(authnRequestContext.RelyingState, authnRequestContext.Destination, new SAMLForm())
        {
            this.RequestContext = authnRequestContext;
        }
        public RequestContext RequestContext { get; }
    }
}