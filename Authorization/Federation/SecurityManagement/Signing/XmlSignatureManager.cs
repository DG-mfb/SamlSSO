using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using Kernel.Cryptography.Signing.Xml;

namespace SecurityManagement.Signing
{
    internal class XmlSignatureManager : IXmlSignatureManager
    {
        public void WriteSignature(XmlDocument xmlElement, string referenceId, AsymmetricAlgorithm signingKey, string digestMethod, string signatureMethod, string inclusiveNamespacesPrefixList = null)
        {
            this.SignXml(xmlElement, referenceId, signingKey, inclusiveNamespacesPrefixList);
        }

        public void SignXml(XmlDocument xmlDoc, string referenceId, AsymmetricAlgorithm key, string inclusiveNamespacesPrefixList)
        {
            // Check arguments.
            if (xmlDoc == null)
                throw new ArgumentException("xmlDocument");
            if (key == null)
                throw new ArgumentException("asymmetricAlgorithm");

            // Create a SignedXml object.
            var signedXml = new SignedXml(xmlDoc);

            // Add the key to the SignedXml document.
            signedXml.SigningKey = key;

            // Create a reference to be signed.
            var reference = new Reference();

            reference.Uri = "#" + referenceId;
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            
            // Add an enveloped transformation to the reference.
            var env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);
            // Add an C14N transformation to the reference.
            var cn14 = new XmlDsigExcC14NTransform(false);
            if(!String.IsNullOrWhiteSpace(inclusiveNamespacesPrefixList))
                cn14.InclusiveNamespacesPrefixList= inclusiveNamespacesPrefixList;
            reference.AddTransform(cn14);

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            // Append the element to the XML document.
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
        }

        public bool VerifySignature(XmlDocument xmlDoc, XmlElement signature, AsymmetricAlgorithm key)
        {
            if (xmlDoc == null)
                throw new ArgumentNullException("xmlDocument");
            if (signature == null)
                throw new ArgumentNullException("signature");
            if (key == null)
                throw new ArgumentNullException("asymmetricAlgorithm");

            var signedXml = new SignedXml(xmlDoc.DocumentElement);
            signedXml.LoadXml(signature);
            return signedXml.CheckSignature(key);
        }

        public bool VerifySignature(XmlDocument xmlDoc, AsymmetricAlgorithm key)
        {
            if (xmlDoc == null)
                throw new ArgumentNullException("xmlDocument");
            if (key == null)
                throw new ArgumentNullException("asymmetricAlgorithm");

            var signEl = xmlDoc.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#")
               .Cast<XmlElement>()
               .First(x => x.ParentNode == xmlDoc.DocumentElement);
            return this.VerifySignature(xmlDoc, signEl, key);
        }
    }
}