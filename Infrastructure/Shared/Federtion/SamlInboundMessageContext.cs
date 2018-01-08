using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using Kernel.Federation.Protocols;
using Shared.Federtion.Models;

namespace Shared.Federtion
{
    public class SamlInboundMessageContext
    {
        private bool _isValid;

        public SamlInboundMessageContext()
        {
            this.Keys = new List<KeyDescriptor>();
            this.Invalidate();
        }
        public bool IsValidated { get { return this._isValid; } }
          
        public Uri OriginUrl { get; set; }

        public string SamlMassage {
            get
            {
                return this.SamlInboundMessage.SamlMessage;
            }
        }
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