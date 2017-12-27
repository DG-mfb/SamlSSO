using System;
using System.Collections.Generic;
using Kernel.Federation.Protocols;
using Shared.Federtion.Forms;

namespace Federation.Protocols.Bindings.HttpPost
{
    public class HttpPostContext : BindingContext
    {
        public HttpPostContext(IDictionary<string, object> relayState, Uri destinationUri, SAMLForm form) : base(relayState, destinationUri)
        {
            this.SAMLForm = form;
        }

        internal SAMLForm SAMLForm { get; }
    }

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