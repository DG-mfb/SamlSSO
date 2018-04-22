using System.IdentityModel.Tokens;
using Kernel.Security.CertificateManagement;

namespace SecurityManagement.TokenResolvers
{
    public class X509CertificateStoreTokenResolverCustom : X509CertificateStoreTokenResolver
    {
        private readonly X509CertificateContext _x509CertificateContext;
        private readonly ICertificateManager _certificateManager;
        public X509CertificateStoreTokenResolverCustom(X509CertificateContext x509CertificateContext, ICertificateManager certificateManager) : base(x509CertificateContext.StoreName, x509CertificateContext.StoreLocation)
        {
            this._x509CertificateContext = x509CertificateContext;
            this._certificateManager = certificateManager;
        }

        protected override bool TryResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
        {
            key = (SecurityKey)null;
            var identifierClause = keyIdentifierClause as EncryptedKeyIdentifierClause;
            if (identifierClause != null)
            {
                var encryptingKeyIdentifier = identifierClause.EncryptingKeyIdentifier;

                if (encryptingKeyIdentifier != null && encryptingKeyIdentifier.Count == 0)
                {
                    var certificate = this._certificateManager.GetCertificateFromContext(this._x509CertificateContext);
                    var kic = new X509RawDataKeyIdentifierClause(certificate);
                    encryptingKeyIdentifier.Add(kic);
                    var result = base.TryResolveSecurityKeyCore(keyIdentifierClause, out key);
                    return result;
                }
            }
            return base.TryResolveSecurityKeyCore(keyIdentifierClause, out key);
        }
    }
}