using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace Kernel.Security.CertificateManagement
{
    public interface ICertificateManager
    {
        X509Certificate2 GetCertificate(string path, SecureString password);
        X509Certificate2 GetCertificate(ICertificateStore store);
        bool TryExtractSpkiBlob(X509Certificate2 certificate, out string spkiEncoded);
        string GetSubjectKeyIdentifier(X509Certificate2 certificate);
        string GetCertificateThumbprint(X509Certificate2 certificate);
        ICertificateStore GetStoreFromContext(CertificateContext certContext);
        X509Certificate2 GetCertificateFromContext(CertificateContext certContext);
        string SignToBase64(string dataToSign, CertificateContext certContext);
        bool VerifySignatureFromBase64(string data, string signed, CertificateContext certContext);
        bool TryAddCertificateToStore(string storeName, StoreLocation location, X509Certificate2 certificate, bool createIfNotExist);
    }
}