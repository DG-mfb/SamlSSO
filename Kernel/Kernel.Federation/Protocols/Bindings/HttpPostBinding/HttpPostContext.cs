using System;
using System.Collections.Generic;

namespace Kernel.Federation.Protocols.Bindings.HttpPostBinding
{
    public class HttpPostContext : BindingContext
    {
        public HttpPostContext(IDictionary<string, object> relayState, Uri destinationUri, ISamlForm form) : base(relayState, destinationUri)
        {
            this.SAMLForm = form;
        }

        internal ISamlForm SAMLForm { get; }
    }
}