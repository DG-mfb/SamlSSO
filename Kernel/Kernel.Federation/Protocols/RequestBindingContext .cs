using System;
using System.Collections.Generic;

namespace Kernel.Federation.Protocols
{
    public class RequestBindingContext : BindingContext
    {
        public RequestBindingContext(AuthnRequestContext authnRequestContext)
            :base(authnRequestContext.RelyingState, authnRequestContext.Destination)
        {
            this.AuthnRequestContext = authnRequestContext;
        }
        public AuthnRequestContext AuthnRequestContext { get; }
        
    }
}