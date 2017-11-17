using System;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using Kernel.Security.Validation;

namespace Federation.Protocols.Test.Mock
{
    internal class CertificateValidatorMock : X509CertificateValidator, ICertificateValidator
    {
        public string FederationPartyId { get; }

        public X509CertificateValidationMode X509CertificateValidationMode
        {
            get
            {
                return X509CertificateValidationMode.None;
            }
        }

        public void SetFederationPartyId(string federationPartyId)
        {
            
        }

        public override void Validate(X509Certificate2 certificate)
        {
            
        }
    }
}