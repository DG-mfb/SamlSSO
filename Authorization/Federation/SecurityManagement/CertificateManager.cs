using System;
using System.IdentityModel.Tokens;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Kernel.Cryptography.DataProtection;
using Kernel.Federation.MetaData.Configuration.Cryptography;
using Kernel.Logging;
using Kernel.Security.CertificateManagement;
using Kernel.Security.Validation;
using SecurityManagement.CerificateContext;
using SecurityManagement.TokenResolvers;

namespace SecurityManagement
{
    public class CertificateManager : ICertificateManager
    {
        internal static Func<ICertificateValidator> CertificateValidatorFactory { private get; set; } = () => new DefaultCertificateValidator();

        private readonly ILogProvider _logProvider;
        private ICertificateValidator _certificateValidator;
        public ICertificateValidator CertificateValidator
        {
            get
            {
                return this._certificateValidator ?? CertificateManager.CertificateValidatorFactory();
            }
            set
            {
                this._certificateValidator = value;
            }
        }

        public CertificateManager(ILogProvider logProvider)
        {
            this._logProvider = logProvider;
        }

        /// <summary>
        /// Try to add a certifictae to a store in given location. Optionally it creates the store if it doesn't exist
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="location"></param>
        /// <param name="certificate"></param>
        /// <param name="createIfNotExist"></param>
        /// <returns></returns>
        public bool TryAddCertificateToStore(string storeName, StoreLocation location, X509Certificate2 certificate, bool createIfNotExist)
        {
            try
            {
                using (var store = new X509Store(storeName, location))
                {
                    var flags = OpenFlags.ReadWrite;
                    if (!createIfNotExist)
                        flags &= OpenFlags.OpenExistingOnly;
                    store.Open(flags);
                    store.Add(certificate);
                    return true;
                }
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Instantiate X509Certificate2 from a certifictae file and password
        /// </summary>
        /// <param name="path"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public X509Certificate2 GetCertificate(string path, SecureString password)
        {
            return new X509Certificate2(path, password);
        }

        /// <summary>
        /// Instantiate X509Certificate2 from a given certificate store
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public X509Certificate2 GetCertificate(ICertificateStore store)
        {
            if (store == null)
                throw new ArgumentNullException("store");
            return store.GetX509Certificate2();
        }

        /// <summary>
        /// Instantiate X509Certificate2 from certificate context. 
        /// </summary>
        /// <param name="certContext"></param>
        /// <returns></returns>
        public X509Certificate2 GetCertificateFromContext(CertificateContext certContext)
        { 
            var store = this.GetStoreFromContext(certContext);
            return this.GetCertificate(store);
        }

        /// <summary>
        /// Get certificate thumbprint
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public string GetCertificateThumbprint(X509Certificate2 certificate)
        {
            if (certificate == null)
                throw new ArgumentNullException("certificate");
            return certificate.Thumbprint;
        }

        /// <summary>
        /// Get x509 store from certificate context. Only x509 store supported.
        /// </summary>
        /// <param name="certContext"></param>
        /// <returns></returns>
        public ICertificateStore GetStoreFromContext(CertificateContext certContext)
        {
            var sb = new StringBuilder();
            var x509Context = certContext as X509CertificateContext;
            if (x509Context == null)
            {
                sb.AppendFormat("Certificate context of type: {0} is not supported.", certContext.GetType().Name);
                throw new NotSupportedException(sb.ToString());
            }

            sb.AppendLine("Try to get certificate store from context. Certificate context details:");
            sb.AppendLine(x509Context.ToString());
            this._logProvider.LogMessage(sb.ToString());
            
            return new X509StoreCertificateConfiguration(x509Context);
        }

        /// <summary>
        /// Sign data and encode it as base64
        /// </summary>
        /// <param name="dataToSign"></param>
        /// <param name="certContext"></param>
        /// <returns></returns>
        public string SignToBase64(string dataToSign, CertificateContext certContext)
        {
            this._logProvider.LogMessage(String.Format("Signing data with certificate from context: {0}", certContext.ToString()));
            var data = Encoding.UTF8.GetBytes(dataToSign);
            var cert = this.GetCertificateFromContext(certContext);
            var signed = this.SignData(dataToSign, cert);

            var base64 = Convert.ToBase64String(signed);
            return base64;
        }
        public byte[] SignData(string dataToSign, X509Certificate2 certificate)
        {
            var data = Encoding.UTF8.GetBytes(dataToSign);
            var signed = RSADataProtection.SignDataSHA1((RSA)certificate.PrivateKey, data);
            return signed;
        }

        /// <summary>
        /// Varify signature
        /// </summary>
        /// <param name="data"></param>
        /// <param name="signed"></param>
        /// <param name="certContext"></param>
        /// <returns></returns>
        public bool VerifySignatureFromBase64(string data, string signed, CertificateContext certContext)
        { 
            var cert = this.GetCertificateFromContext(certContext);
            return this.VerifySignatureFromBase64(data, signed, cert);
        }

        public bool VerifySignatureFromBase64(string data, string signed, X509Certificate2 certificate)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signedBytes = Convert.FromBase64String(signed);
            var verified = RSADataProtection.VerifyDataSHA1Signed((RSA)certificate.PublicKey.Key, dataBytes, signedBytes);
            return verified;
        }

        /// <summary>
        /// Try to extract subject public key information from certificate
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="spkiEncoded"></param>
        /// <returns></returns>
        public bool TryExtractSpkiBlob(X509Certificate2 certificate, out string spkiEncoded)
        {
            try
            {
                var spki = Utility.ExtractSpkiBlob(certificate);
                spkiEncoded = Utility.HashSpki(spki);
                return !String.IsNullOrWhiteSpace(spkiEncoded);
            }
            catch(Exception ex)
            {
                Exception innerEx;
                this._logProvider.TryLogException(ex, out innerEx);
                spkiEncoded = null;
                return false;
            }
        }

        /// <summary>
        /// Get subject identifier from the certificate
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public string GetSubjectKeyIdentifier(X509Certificate2 certificate)
        {
            if (certificate == null)
                throw new ArgumentNullException("certificate");
            return Utility.GetSubjectKeyIdentifier(certificate);
        }

        public X509CertificateStoreTokenResolver GetX509CertificateStoreTokenResolver(X509CertificateContext x509CertificateContext)
        {
            return new X509CertificateStoreTokenResolverCustom(x509CertificateContext, this);
        }
    }
}