using System.Security.Cryptography.X509Certificates;

namespace SecurityManagement.Tests.Mock
{
    internal class CertificateProviderMock
    {
        private static X509Certificate2 mockCert;

        public static X509Certificate2 GetMockCertificate()
        {
            if (CertificateProviderMock.mockCert == null)
            {
                using (var store = new X509Store("testCertStore", StoreLocation.LocalMachine))
                {
                    try
                    {
                        store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                        var certSource = store.Certificates.Find(X509FindType.FindBySubjectName, "www.eca-international.com", false)[0];
                        CertificateProviderMock.mockCert = certSource;
                    }
                    finally
                    {
                        store.Close();
                    }
                }
            }
            return CertificateProviderMock.mockCert;
        }
    }
}