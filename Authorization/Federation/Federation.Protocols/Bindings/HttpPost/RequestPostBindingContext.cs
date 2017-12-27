using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using Shared.Federtion.Forms;

namespace Federation.Protocols.Bindings.HttpPost
{
    public class RequestPostBindingContext : HttpPostContext
    {
        public RequestPostBindingContext(AuthnRequestContext authnRequestContext)
            : base(authnRequestContext.RelyingState, authnRequestContext.Destination, new SAMLForm())
        {
            this.AuthnRequestContext = authnRequestContext;
        }
        public AuthnRequestContext AuthnRequestContext { get; }

    }
}