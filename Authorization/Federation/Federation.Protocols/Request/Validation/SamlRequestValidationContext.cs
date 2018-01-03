using System;
using System.Collections.Generic;
using Kernel.Federation.Protocols;
using Kernel.Validation;

namespace Federation.Protocols.Request.Validation
{
    internal class SamlRequestValidationContext : ValidationContext
    {
        public IDictionary<string, string> Form { get; }
        public SamlInboundContext InboundContext { get { return (SamlInboundContext)base.Entry; } }
        public SamlRequestValidationContext(SamlInboundContext entry, IDictionary<string, string> form) : this((object)entry)
        {
            this.Form = form;
        }

        protected SamlRequestValidationContext(object entry) : base(entry)
        {
        }

        protected override object GetServiceInternal(Type serviceType)
        {
            return base.GetServiceInternal(serviceType);
        }
    }
}