using System;
using Kernel.Validation;
using Shared.Federtion.Request;

namespace Federation.Protocols.Request.Validation
{
    internal class SamlRequestValidationContext : ValidationContext
    {
        public SamlInboundRequestContext RequestContext { get { return (SamlInboundRequestContext)base.Entry; } }
        public SamlRequestValidationContext(SamlInboundRequestContext context) : this((object)context)
        {
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