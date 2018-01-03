using System;
using System.Collections.Generic;
using Kernel.Federation.Protocols;
using Kernel.Validation;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request.Validation
{
    internal class SamlRequestValidationContext : ValidationContext
    {
        public IDictionary<string, string> Form
        {
            get
            {
                return  this.InboundContext.Form;
            }
        }
        public SamlInboundContext InboundContext { get; } 
        public RequestAbstract Request { get { return (RequestAbstract)base.Entry; } }
        public SamlRequestValidationContext(SamlInboundContext context, RequestAbstract request) : this((object)request)
        {
            this.InboundContext = context;
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