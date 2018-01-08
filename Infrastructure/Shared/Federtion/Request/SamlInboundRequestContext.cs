using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using Kernel.Federation.Protocols;
using Shared.Federtion.Models;

namespace Shared.Federtion.Request
{
    public class SamlInboundRequestContext
    {
        private bool _isValid;

        public SamlInboundRequestContext()
        {
            this.Keys = new List<KeyDescriptor>();
            this.Invalidate();
        }
        public bool IsValidated { get { return this._isValid; } }
        public RequestAbstract SamlRequest { get; set; }   
        public Uri Request { get; set; }
        public SamlInboundMessage SamlInboundMessage { get; set; }
        public ICollection<KeyDescriptor> Keys { get; }

        public void Validated()
        {
            this._isValid = true;
        }
        public void Invalidate()
        {
            this._isValid = false;
        }
    }
}