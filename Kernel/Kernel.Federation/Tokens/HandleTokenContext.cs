using System.Collections.Generic;
using System.Xml;

namespace Kernel.Federation.Tokens
{
    public class HandleTokenContext
    {
        public XmlElement Token { get; }
        public string _federationPartyId { get; }
        public string AuthenticationMethod { get; }
        public object RelayState { get; }
        public string Origin
        {
            get
            {
                var rs = this.RelayState as IDictionary<string, object>;
                if (rs == null)
                    return null;
                return rs["origin"].ToString();
            }
        }
        public HandleTokenContext(XmlElement token, string federationPartyId, string authenticationMethod, object relayState)
        {
            this.Token = token;
            this._federationPartyId = federationPartyId;
            this.AuthenticationMethod = authenticationMethod;
            this.RelayState = relayState;
        }
    }
}