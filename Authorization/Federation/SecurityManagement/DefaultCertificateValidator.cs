using System;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using Kernel.Security.Validation;

namespace SecurityManagement
{
    internal class DefaultCertificateValidator : X509CertificateValidator, ICertificateValidator
    {
        private readonly X509CertificateValidator _innerCertificateValidator;

        public DefaultCertificateValidator()
        {
            this._innerCertificateValidator = X509CertificateValidator.ChainTrust;
        }

        public string FederationPartyId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public X509CertificateValidationMode X509CertificateValidationMode
        {
            get
            {
                return X509CertificateValidationMode.ChainTrust;
            }
        }
        
        public void SetFederationPartyId(string federationPartyId)
        {
            throw new NotImplementedException();
        }

        public override void Validate(X509Certificate2 certificate)
        {
            this._innerCertificateValidator.Validate(certificate);
        }
    }
}