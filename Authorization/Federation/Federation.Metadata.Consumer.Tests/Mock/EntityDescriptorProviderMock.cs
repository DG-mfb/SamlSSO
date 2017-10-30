using System;
using System.IdentityModel.Metadata;
using System.Security.Cryptography.X509Certificates;
using Shared.Federtion.Constants;

namespace Federation.Metadata.Consumer.Tests.Mock
{
    public class EntityDescriptorProviderMock
    {
        private static X509Certificate2 mockCert;
        public static EntityDescriptor GetIdpEntityDescriptor()
        {
            var descriptor = new EntityDescriptor();
            var cert = EntityDescriptorProviderMock.GetMockCertificate();
            var idpRole = new IdentityProviderSingleSignOnDescriptor();
            idpRole.SingleSignOnServices.Add(new ProtocolEndpoint(new Uri(ProtocolBindings.HttpRedirect), new Uri("http://localhost:60879")));
            descriptor.RoleDescriptors.Add(idpRole);
            return descriptor;
        }

        public static EntityDescriptor GetSpEntityDescriptor()
        {
            var descriptor = new EntityDescriptor();
            var cert = EntityDescriptorProviderMock.GetMockCertificate();
            var idpRole = new ServiceProviderSingleSignOnDescriptor();
            idpRole.AssertionConsumerServices.Add(0, new IndexedProtocolEndpoint(0, new Uri(ProtocolBindings.HttpRedirect), new Uri("http://localhost:60879")));
            descriptor.RoleDescriptors.Add(idpRole);
            return descriptor;
        }

        public static EntitiesDescriptor GetEntitiesDescriptor(int idpEntities, int spEntities)
        {
            var descriptor = new EntitiesDescriptor();
            for (var i = 0; i < idpEntities; i++)
            {
                var entityDesc = EntityDescriptorProviderMock.GetIdpEntityDescriptor();
                descriptor.ChildEntities.Add(entityDesc);
            }

            for (var i = 0; i < spEntities; i++)
            {
                var entityDesc = EntityDescriptorProviderMock.GetSpEntityDescriptor();
                descriptor.ChildEntities.Add(entityDesc);
            }
            return descriptor;
        }

        public static X509Certificate2 GetMockCertificate()
        {
            if (EntityDescriptorProviderMock.mockCert == null)
            {
                using (var store = new X509Store(StoreName.My))
                {
                    try
                    {
                        store.Open(OpenFlags.ReadOnly);
                        var certSource = store.Certificates[0];
                        var rawData = certSource.GetRawCertData();
                        var cert = new X509Certificate2(rawData);
                        EntityDescriptorProviderMock.mockCert = cert;
                    }
                    finally
                    {
                        store.Close();
                    }
                }
            }
            return EntityDescriptorProviderMock.mockCert;
        }
    }
}