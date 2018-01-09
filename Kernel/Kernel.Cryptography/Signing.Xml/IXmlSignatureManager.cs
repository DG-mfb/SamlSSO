using System.Security.Cryptography;
using System.Xml;

namespace Kernel.Cryptography.Signing.Xml
{
    public interface IXmlSignatureManager
    {
        void WriteSignature(XmlDocument xmlElement, string referenceId, AsymmetricAlgorithm signingKey, string digestMethod, string signatureMethod);
        bool VerifySignature(XmlDocument xmlDoc, XmlElement signature, AsymmetricAlgorithm key);
    }
}