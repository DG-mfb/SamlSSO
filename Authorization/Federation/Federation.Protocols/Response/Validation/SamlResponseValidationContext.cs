using System;
using Kernel.Validation;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response.Validation
{
    internal class SamlResponseValidationContext : ValidationContext
    {
        public SamlInboundResponseContext ResponseContext { get { return (SamlInboundResponseContext)base.Entry; } }
        public SamlResponseValidationContext(SamlInboundResponseContext entry) : this((object)entry)
        {
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