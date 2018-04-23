﻿using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.Tokens;
using Kernel.Security.CertificateManagement;
using Kernel.Security.Validation;
using Shared.Federtion;

namespace Federation.Protocols.Tokens
{
    internal class TokenHandlerConfigurationProvider : ITokenConfigurationProvider<SecurityTokenHandlerConfiguration>
    {
        private readonly IAssertionPartyContextBuilder _federationPartyContextBuilder;
        private readonly ICertificateValidator _certificateValidator;
        private readonly ICertificateManager certificateManager;
        public TokenHandlerConfigurationProvider(IAssertionPartyContextBuilder federationPartyContextBuilder, ICertificateManager certificateManager)
        {
            this._federationPartyContextBuilder = federationPartyContextBuilder;
            this._certificateValidator = certificateManager.CertificateValidator;
            this.certificateManager = certificateManager;
        }
        
        public SecurityTokenHandlerConfiguration GetConfiguration(string partnerId)
        {
            this._certificateValidator.SetFederationPartyId(partnerId);

            var partnerContex = this._federationPartyContextBuilder.BuildContext(partnerId);
            var descriptor = partnerContex.MetadataContext.EntityDesriptorConfiguration.SPSSODescriptors.First();
            var cert = descriptor.KeyDescriptors.First(x => x.IsDefault && x.Use == Kernel.Federation.MetaData.Configuration.Cryptography.KeyUsage.Encryption);
            if (cert.CertificateContext == null)
                throw new ArgumentNullException("certificate context");

            var x509CertificateContext = cert.CertificateContext as X509CertificateContext;
            if (x509CertificateContext == null)
                throw new InvalidOperationException(String.Format("Expected certificate context of type: {0} but it was:{1}", typeof(X509CertificateContext).Name, cert.CertificateContext.GetType()));

            var inner = this.certificateManager.GetX509CertificateStoreTokenResolver(x509CertificateContext);
            var tokenResolver = new IssuerTokenResolver(inner);
            var issuerNameRegistry = IdentityConfigurationHelper.IssuerNameRegistry;
            var configuration = new SecurityTokenHandlerConfiguration
            {
                IssuerNameRegistry = issuerNameRegistry,
                IssuerTokenResolver = tokenResolver,
                ServiceTokenResolver = inner,
                AudienceRestriction = new AudienceRestriction(AudienceUriMode.Always),
                CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom,
                CertificateValidator = (X509CertificateValidator)this._certificateValidator
            };
            
            configuration.AudienceRestriction.AllowedAudienceUris.Add(new Uri(partnerContex.MetadataContext.EntityId));
            
            return configuration;
        }

        public SecurityTokenHandlerConfiguration GetTrustedIssuersConfiguration()
        {
            var issuerNameRegistry = IdentityConfigurationHelper.IssuerNameRegistry;
            var configuration = new SecurityTokenHandlerConfiguration
            {
                IssuerNameRegistry = issuerNameRegistry
            };
            return configuration;
        }
    }
}