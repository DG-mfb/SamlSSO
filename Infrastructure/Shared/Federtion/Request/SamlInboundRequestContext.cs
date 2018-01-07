using System;
using System.Collections.Generic;
using System.Text;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;
using Shared.Federtion.Models;

namespace Shared.Federtion.Request
{
    public class SamlInboundRequestContext
    {
        public SamlInboundRequestContext()
        {
        }
        //public string FederationPartyId
        //{
        //    get
        //    {
        //        if (this.SamlInboundMessage == null)
        //            throw new ArgumentNullException("SamlInboundMessage");

        //        IDictionary<string, object> relayStateDictionary;
        //        if (!this.SamlInboundMessage.TryGetRelayState(out relayStateDictionary))
        //            throw new InvalidOperationException(String.Format("Expected relay state type of: {0}", typeof(IDictionary<string, object>).Name));
        //        object partnerId;
        //        if (relayStateDictionary.TryGetValue(RelayStateContstants.FederationPartyId, out partnerId))
        //            return partnerId.ToString();
        //        return null;
        //    }
        //}
        public RequestAbstract SamlRequest { get; set; }
        
        public string Request { get; set; }
        public SamlInboundMessage SamlInboundMessage { get; set; }

    }
}