using System;
using System.Xml.Serialization;
using Shared.Federtion.Constants;

namespace Shared.Federtion.Response
{
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class LogoutResponse : StatusResponse
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "LogoutResponse";
    }
}