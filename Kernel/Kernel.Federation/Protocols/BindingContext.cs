using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kernel.Federation.Protocols
{
    public class BindingContext
    {
        public BindingContext(IDictionary<string, object> relayState, Uri destinationUri)
        {
            this.DestinationUri = destinationUri;
            this.RelayState = relayState;
            this.RequestParts = new Dictionary<string, string>();
        }
        public Uri DestinationUri { get; }
        public IDictionary<string, object> RelayState { get; }
        public IDictionary<string, string> RequestParts { get; set; }
        public virtual Uri GetDestinationUrl()
        {
            return this.DestinationUri;
        }
    }
}