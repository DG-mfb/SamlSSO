using System;
using System.Xml;
using System.Xml.Serialization;
using Shared.Federtion.Constants;

namespace Shared.Federtion.Response
{
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class TokenResponse : StatusResponse
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "Response";

        #region Elements

        /// <summary>
        /// Gets or sets the items.
        /// Specifies an assertion by value, or optionally an encrypted assertion by value.
        /// </summary>
        /// <value>The items.</value>
        [XmlElement("Assertion", Namespace = Saml20Constants.Assertion, Order = 1)]
        //[XmlElement("EncryptedAssertion", Namespace = Saml20Constants.Assertion, Order = 1)]
        public XmlElement[] Assertions { get; set; }

        #endregion
    }
}
