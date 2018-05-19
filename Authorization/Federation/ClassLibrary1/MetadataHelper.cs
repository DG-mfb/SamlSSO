using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Security.Cryptography.X509Certificates;
using Kernel.Federation.Constants;
using Kernel.Federation.MetaData.Configuration;
using Kernel.Federation.MetaData.Configuration.Cryptography;
using Kernel.Federation.MetaData.Configuration.EndPoint;
using Kernel.Federation.MetaData.Configuration.EntityDescriptors;
using Kernel.Federation.MetaData.Configuration.Organisation;
using Kernel.Federation.MetaData.Configuration.RoleDescriptors;
using Kernel.Security.CertificateManagement;

namespace InlineMetadataContextProvider
{
    internal class MetadataHelper
    {
        public static EntityDesriptorConfiguration BuildEntityDesriptorConfiguration(bool includeOrganisation = false)
        {
            var federationId = String.Format("{0}_{1}", "IdpFederation", Guid.NewGuid());
            var entityDescriptorConfiguration = new EntityDesriptorConfiguration
            {
                CacheDuration = TimeSpan.FromDays(100),
                EntityId = "https://www.eca-international.com",
                Id = federationId,
                ValidUntil = new DateTimeOffset(DateTime.Now.AddDays(30)),
            };
            if (includeOrganisation)
                entityDescriptorConfiguration.Organisation = MetadataHelper.BuildOrganisationConfiguration();

            return entityDescriptorConfiguration;
        }

        public static OrganisationConfiguration BuildOrganisationConfiguration()
        {
            var orgConfiguration = new OrganisationConfiguration
            {
                OrganisationContacts = new ContactConfiguration()
            };
            orgConfiguration.Names.Add(new LocalizedConfigurationEntry
            {
                Name = "Company name",
                DisplayName = "Company Display name",
            });
            orgConfiguration.Urls.Add(new LocalizedUrlEntry { Url = new Uri("https://company.co.uk/") });

            var contact = new Kernel.Federation.MetaData.Configuration.Organisation.ContactPerson
            {
                ContactType = Kernel.Federation.MetaData.Configuration.Organisation.ContactType.Technical,
                ForeName = "John",
                SurName = "Doe",

            };
            contact.Emails.Add("john.doe@domain.co.uk");
            contact.PhoneNumbers.Add("020123456");
            
            orgConfiguration.OrganisationContacts.PersonContact.Add(contact);
            return orgConfiguration;
        }

        public static IEnumerable<KeyDescriptorConfiguration> BuildKeyDescriptorConfiguration()
        {
            var certificateContext = new X509CertificateContext
            {
                StoreName = "TestCertStore",
                ValidOnly = false,
                StoreLocation = StoreLocation.LocalMachine
            };
            certificateContext.SearchCriteria.Add(new CertificateSearchCriteria { SearchValue = "www.eca-international.com", SearchCriteriaType = X509FindType.FindBySubjectName });
            var keyDescriptorConfiguration = new KeyDescriptorConfiguration
            {
                IsDefault = true,
                Use = KeyUsage.Signing,
                CertificateContext = certificateContext
            };

            var encrCertificateContext = new X509CertificateContext
            {
                StoreName = "TestCertStore",
                ValidOnly = false,
                StoreLocation = StoreLocation.LocalMachine
            };
            encrCertificateContext.SearchCriteria.Add(new CertificateSearchCriteria { SearchValue = "www.eca-international.com", SearchCriteriaType = X509FindType.FindBySubjectName });
            var encrKeyDescriptorConfiguration = new KeyDescriptorConfiguration
            {
                IsDefault = true,
                Use = KeyUsage.Encryption,
                CertificateContext = certificateContext
            };
            return new[] { keyDescriptorConfiguration, encrKeyDescriptorConfiguration };
        }

        public static SPSSODescriptorConfiguration BuildSPSSODescriptorConfiguration()
        {
            var sPSSODescriptorConfiguration = new SPSSODescriptorConfiguration
            {
                WantAssertionsSigned = true,
                ValidUntil = new DateTimeOffset(DateTime.Now.AddDays(100)),
                Organisation = MetadataHelper.BuildOrganisationConfiguration(),
                AuthenticationRequestsSigned = true,
                CacheDuration = TimeSpan.FromDays(100),
                RoleDescriptorType = typeof(ServiceProviderSingleSignOnDescriptor),
                ErrorUrl = new Uri("http://localhost:60879/api/Account/Error")
            };
            sPSSODescriptorConfiguration.NameIdentifierFormats.Add(new Uri("urn:oasis:names:tc:SAML:2.0:nameid-format:transient"));
            sPSSODescriptorConfiguration.NameIdentifierFormats.Add(new Uri("urn:oasis:names:tc:SAML:2.0:nameid-format:persistent"));
            sPSSODescriptorConfiguration.SingleLogoutServices.Add(new EndPointConfiguration
            {
                Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"),
                Location = new Uri("http://localhost:60879/api/Account/SSOLogout")
            });
            //supported protocols
            sPSSODescriptorConfiguration.ProtocolSupported.Add(new Uri("urn:oasis:names:tc:SAML:2.0:protocol"));
            //key descriptors
            var keyDescriptorConfiguration = MetadataHelper.BuildKeyDescriptorConfiguration();
            foreach (var k in keyDescriptorConfiguration)
            {
                sPSSODescriptorConfiguration.KeyDescriptors.Add(k);
            }

            //assertinon service
            var indexedEndPointConfiguration = new IndexedEndPointConfiguration
            {
                Index = 0,
                IsDefault = true,
                Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"),
                Location = new Uri("http://localhost:60879/api/Account/SSOLogon")
            };
            sPSSODescriptorConfiguration.AssertionConsumerServices.Add(indexedEndPointConfiguration);

            return sPSSODescriptorConfiguration;
        }

        public static IdPSSODescriptorConfiguration BuildIdPSSODescriptorConfiguration()
        {
            var idPSSODescriptorConfiguration = new IdPSSODescriptorConfiguration
            {
                ValidUntil = new DateTimeOffset(DateTime.Now.AddDays(100)),
                Organisation = MetadataHelper.BuildOrganisationConfiguration(),
                WantAuthenticationRequestsSigned = true,
                CacheDuration = TimeSpan.FromDays(100),
                RoleDescriptorType = typeof(IdentityProviderSingleSignOnDescriptor),
                ErrorUrl = new Uri("http://localhost:60879/api/Account/Error")
            };
            idPSSODescriptorConfiguration.NameIdentifierFormats.Add(new Uri("urn:oasis:names:tc:SAML:2.0:nameid-format:transient"));
            idPSSODescriptorConfiguration.NameIdentifierFormats.Add(new Uri("urn:oasis:names:tc:SAML:2.0:nameid-format:persistent"));
            idPSSODescriptorConfiguration.SingleLogoutServices.Add(new EndPointConfiguration
            {
                Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"),
                Location = new Uri("http://localhost:60879/api/Account/SSOLogout")
            });
            //supported protocols
            idPSSODescriptorConfiguration.ProtocolSupported.Add(new Uri("urn:oasis:names:tc:SAML:2.0:protocol"));
            //key descriptors
            var keyDescriptorConfiguration = MetadataHelper.BuildKeyDescriptorConfiguration();
            foreach (var k in keyDescriptorConfiguration)
            {
                idPSSODescriptorConfiguration.KeyDescriptors.Add(k);
            }

            //assertinon service
            var endPointConfiguration = new EndPointConfiguration
            {
                Binding = new Uri(ProtocolBindings.HttpRedirect),
                Location = new Uri("https://localhost:44342/sso/login")
            };
            idPSSODescriptorConfiguration.SignOnServices.Add(endPointConfiguration);

            return idPSSODescriptorConfiguration;
        }
    }
}