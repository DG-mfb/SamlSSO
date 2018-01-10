using System;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Federation.Protocols.Tokens;
using Kernel.Compression;
using Kernel.Cryptography.Signing.Xml;
using Kernel.Federation.Protocols;
using Kernel.Security.CertificateManagement;
using Shared.Federtion;

namespace Federation.Protocols
{
    internal class Helper
    {
        internal static async Task<string> DeflateEncode(string val, ICompression compression)
        {
            var strArr = Encoding.UTF8.GetBytes(val);
            using (var memoryStream = new MemoryStream(strArr))
            {
                using (var compressed = new MemoryStream())
                {
                    await compression.Compress(memoryStream, compressed);
                    compressed.Position = 0;
                    return Convert.ToBase64String(compressed.GetBuffer(), 0, (int)compressed.Length, Base64FormattingOptions.None);
                }
            }
        }

        internal static async Task<string> DeflateDecompress(string value, ICompression compression)
        {
            var encoded = Convert.FromBase64String(value);
            using (var compressed = new MemoryStream(encoded))
            {
                using (var decompressed = new MemoryStream())
                {
                    await compression.Decompress(compressed, decompressed);
                    decompressed.Position = 0;
                    using (var streamReader = new StreamReader(decompressed, Encoding.UTF8))
                    {
                        return await streamReader.ReadToEndAsync();
                    }
                }
            }
        }

        internal static bool VerifyRedirectSignature(Uri request, X509Certificate2 certificate, SamlInboundMessage message, ICertificateManager certificateManager)
        {
            var queryString = request.Query.TrimStart('?');
            var i = queryString.IndexOf("Signature");
            if (i == -1)
                throw new InvalidOperationException("No signature found.");
            var data = queryString.Substring(0, i - 1);
            var sgn = message.Signature.Signature;

            var validated = certificateManager.VerifySignatureFromBase64(data, sgn, certificate);
            return validated;
        }
        public static bool ValidateRedirectSignature(SamlInboundMessageContext inboundContext, ICertificateManager certificateManager)
        {
            var validated = false;
            foreach (var k in inboundContext.Keys.SelectMany(x => x.KeyInfo))
            {
                var binaryClause = k as BinaryKeyIdentifierClause;
                if (binaryClause == null)
                    throw new InvalidOperationException(String.Format("Expected type: {0} but it was: {1}", typeof(BinaryKeyIdentifierClause), k.GetType()));

                var certContent = binaryClause.GetBuffer();
                var cert = new X509Certificate2(certContent);
                validated = Helper.VerifyRedirectSignature(inboundContext.OriginUrl, cert, inboundContext.SamlInboundMessage, certificateManager);
                if (validated)
                    break;
            }
            return validated;
        }

        public static bool ValidateMessageSignature(SamlInboundMessageContext inboundContext, IXmlSignatureManager signatureManager)
        {
            var validated = false;
            var cspParams = new CspParameters();
            cspParams.KeyContainerName = "XML_DSIG_RSA_KEY";
            var rsaKey = new RSACryptoServiceProvider(cspParams);
            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(inboundContext.SamlMassage);

            var signEl = TokenHelper.GetAllElements("Signature", "http://www.w3.org/2000/09/xmldsig#", doc.DocumentElement)
                .FirstOrDefault(x => x.ParentNode == doc.DocumentElement);
            if (signEl == null)
                return true;
            
            foreach (var k in inboundContext.Keys.SelectMany(x => x.KeyInfo))
            {
                var binaryClause = k as BinaryKeyIdentifierClause;
                if (binaryClause == null)
                    throw new InvalidOperationException(String.Format("Expected type: {0} but it was: {1}", typeof(BinaryKeyIdentifierClause), k.GetType()));

                var certContent = binaryClause.GetBuffer();
                var cert = new X509Certificate2(certContent);
                validated = signatureManager.VerifySignature(doc, signEl,cert.PublicKey.Key);
                if (validated)
                    break;
            }

            if (!validated)
            {
                var certEl = TokenHelper.GetElement("X509Certificate", "http://www.w3.org/2000/09/xmldsig#", signEl);
                if (certEl != null)
                {
                    var dcert2 = new X509Certificate2(Convert.FromBase64String(certEl.InnerText));
                    validated = signatureManager.VerifySignature(doc, signEl, dcert2.PublicKey.Key);//signedXml.CheckSignature(dcert2, true);
                }
            }
            return validated;
        }
    }
}