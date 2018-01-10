using System.Security.Cryptography;
using System.Xml;

namespace Kernel.Cryptography.Signing.Xml
{
    public interface IXmlSignatureManager
    {
        void WriteSignature(XmlDocument xmlElement, string referenceId, AsymmetricAlgorithm signingKey, string digestMethod, string signatureMethod, string inclusiveNamespacesPrefixList = null);
        bool VerifySignature(XmlDocument xmlDoc, XmlElement signature, AsymmetricAlgorithm key);
        bool VerifySignature(XmlDocument xmlDoc, AsymmetricAlgorithm key);
    }
}