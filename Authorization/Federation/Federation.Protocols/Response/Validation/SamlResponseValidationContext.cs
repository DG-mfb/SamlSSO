using System;
using System.Collections.Generic;
using Kernel.Validation;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response.Validation
{
    internal class SamlResponseValidationContext : ValidationContext
    {
        public IDictionary<string, string> Form { get; }
        public ResponseStatus Response { get { return (ResponseStatus)base.Entry; } }
        public SamlResponseValidationContext(ResponseStatus entry, IDictionary<string, string> form) : this((object)entry)
        {
            this.Form = form;
        }

        protected SamlResponseValidationContext(object entry) : base(entry)
        {
        }

        protected override object GetServiceInternal(Type serviceType)
        {
            return base.GetServiceInternal(serviceType);
        }
    }
}