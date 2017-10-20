using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Owin.CertificateValidators.Tests
{
    internal class Helper
    {
        internal static IEnumerable<string> GetValidBase64EncodedSubjectPublicKeyInfoHashes(StoreName storeName, StoreLocation location, X509FindType findType, object value, out X509Certificate2 cert)
        {
            using (var store = new X509Store(storeName, location))
            {
                store.Open(OpenFlags.OpenExistingOnly);
                cert = store.Certificates.Find(findType, value, false)[0];
                //ACT
                var pkinfo = Utility.GetSubjectPublicKeyInfo(cert);
                var base64Pkif = Utility.HashSpki(pkinfo);
                return new[] { base64Pkif };
            }
        }
    }
}