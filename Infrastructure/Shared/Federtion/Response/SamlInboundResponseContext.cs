using System;
using System.Collections.Generic;
using System.Text;
using Kernel.Federation.Constants;

namespace Shared.Federtion.Response
{
    public class SamlInboundResponseContext : SamlInboundMessageContext
    {
        public SamlInboundResponseContext()
        {
        }
        public string FederationPartyId
        {
            get
            {
                if (this.SamlInboundMessage == null)
                    throw new ArgumentNullException("SamlInboundMessage");

                IDictionary<string, object> relayStateDictionary;
                if (!this.SamlInboundMessage.TryGetRelayState(out relayStateDictionary))
                    return this.StatusResponse.Issuer.Value;
                object partnerId;
                if (relayStateDictionary.TryGetValue(RelayStateContstants.FederationPartyId, out partnerId))
                    return partnerId.ToString();
                return null;
            }
        }
        public StatusResponse StatusResponse { get; set; }
       
        public bool IsSuccess
        {
            get
            {
                return this.StatusResponse.Status.StatusCode.Value == StatusCodes.Success;
            }
        }
        public bool IsIdpInitiated
        {
            get
            {
                return String.IsNullOrEmpty(this.StatusResponse.InResponseTo);
            }
        }
        
        public string AggregatedMessage
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendFormat("StatusCode: {0}\r\n", this.StatusResponse.Status.StatusCode.Value);
                var subCode = this.StatusResponse.Status.StatusCode.SubStatusCode;
                while (subCode != null)
                {
                    if (!String.IsNullOrWhiteSpace(subCode.Value))
                        sb.AppendFormat("Additional status code: {0}\r\n", subCode.Value);
                    subCode = subCode.SubStatusCode;
                }
                if (!String.IsNullOrWhiteSpace(this.StatusResponse.Status.StatusMessage))
                    sb.AppendFormat("Message status: {0}\r\n", this.StatusResponse.Status.StatusMessage);

                //if (!String.IsNullOrWhiteSpace(this.MessageDetails))
                //    sb.AppendFormat("Message details: {0}", this.MessageDetails);
                
                return sb.ToString();
            }
        }
    }
}