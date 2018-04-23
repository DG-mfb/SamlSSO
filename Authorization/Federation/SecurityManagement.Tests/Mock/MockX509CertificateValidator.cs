using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using Kernel.Security.Validation;

namespace SecurityManagement.Tests.Mock
{
    internal class MockX509CertificateValidator : ICertificateValidator
    {
        public string FederationPartyId { get { throw new NotImplementedException(); } }

        public X509CertificateValidationMode X509CertificateValidationMode { get { throw new NotImplementedException(); } }

        public void SetFederationPartyId(string federationPartyId)
        {
            throw new NotImplementedException();
        }

        public void Validate(X509Certificate2 certificate)
        {
            throw new NotImplementedException();
        }
    }
}
