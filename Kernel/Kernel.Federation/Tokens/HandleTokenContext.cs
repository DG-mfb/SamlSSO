using System.Xml;

namespace Kernel.Federation.Tokens
{
    public class HandleTokenContext
    {
        public XmlReader Token { get; }
        public string _federationPartyId { get; }
        public string AuthenticationMethod { get; }
        public HandleTokenContext(XmlReader token, string federationPartyId, string authenticationMethod)
        {
            this.Token = token;
            this._federationPartyId = federationPartyId;
            this.AuthenticationMethod = authenticationMethod;
        }
    }
}