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
            this.ClauseBuilder = new StringBuilder();
            this.DestinationUri = destinationUri;
            this.RelayState = relayState;
            this.Request = new XmlDocument();
        }
        public Uri DestinationUri { get; }
        public StringBuilder ClauseBuilder { get; }
        public IDictionary<string, object> RelayState { get; }
        public XmlDocument Request { get; set; }
        public virtual Uri GetDestinationUrl()
        {
            return this.DestinationUri;
        }
    }
}