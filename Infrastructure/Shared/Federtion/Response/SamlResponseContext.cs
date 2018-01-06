using System;
using System.Collections.Generic;
using System.Text;
using Shared.Federtion.Constants;

namespace Shared.Federtion.Response
{
    public class SamlResponseContext
    {
        public SamlResponseContext()
        {
        }
        public string FederationPartyId
        {
            get
            {
                if (this.RelayState == null)
                    throw new ArgumentNullException("relay state");

                var relayStateDictionary = this.RelayState as IDictionary<string, object>;
                if (relayStateDictionary == null)
                    throw new InvalidOperationException(String.Format("Expected relay state type of: {0}, but it was: {1}", typeof(IDictionary<string, object>).Name, this.RelayState.GetType().Name));
                object partnerId;
                if (relayStateDictionary.TryGetValue(RelayStateContstants.FederationPartyId, out partnerId))
                    return partnerId.ToString();
                return null;
            }
        }
        public StatusResponse StatusResponse { get; set; }
        
        public string Response { get; set; }
        public object RelayState { get; set; }
       
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