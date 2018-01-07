using System;
using System.Xml;
using System.Xml.Serialization;
using Kernel.Federation.Constants;

namespace Shared.Federtion.Response
{
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class StatusDetail
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "StatusDetail";

        #region Elements

        /// <summary>
        /// Gets or sets the any XML element.
        /// </summary>
        /// <value>The Any XML element.</value>
        [XmlAnyElement(Order = 1)]
        public XmlElement[] Any { get; set; }

        #endregion
    }
}
