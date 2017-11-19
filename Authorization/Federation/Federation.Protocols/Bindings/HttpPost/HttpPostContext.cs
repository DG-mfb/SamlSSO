using Kernel.Federation.Protocols;
using Shared.Federtion.Forms;

namespace Federation.Protocols.Bindings.HttpPost
{
    public class HttpPostContext : RequestBindingContext
    {
        public HttpPostContext(AuthnRequestContext authnRequestContext) : base(authnRequestContext)
        {
        }

        internal SAMLForm SAMLForm { get; }
    }
}