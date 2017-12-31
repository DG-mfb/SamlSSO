using System;
using System.IdentityModel.Metadata;
using System.Linq;
using Federation.Metadata.Consumer.Tests.Mock;
using Federation.Metadata.FederationPartner.Handlers;
using NUnit.Framework;
using Shared.Federtion.Constants;
using Shared.Federtion.Factories;

namespace Federation.Metadata.Consumer.Tests
{
    [TestFixture]
    internal class MetadataHandlerTests
    {
        [Test]
        public void GetDelegateForIdpDescriptors_entity_descriptor_metadata_Test()
        {
            //ARRANGE
            var metadata = EntityDescriptorProviderMock.GetIdpEntityDescriptor("TestEntityId");
            var handler = new MetadataEntitityDescriptorHandler();
            //ACT
            var roleDescriptors = handler.GetRoleDescriptors<IdentityProviderSingleSignOnDescriptor>(metadata)
                .ToList();
            //ASSERT
            Assert.AreEqual(1, roleDescriptors.Count);
            Assert.AreEqual(1, roleDescriptors.Single().Roles.Count);
        }

        [Test]
        public void GetDelegateForIdpDescriptor_entity_descriptor_metadata_Test()
        {
            //ARRANGE
            var metadata = EntityDescriptorProviderMock.GetIdpEntityDescriptor("TestEntityId");
            var handler = new MetadataEntitityDescriptorHandler();
            //ACT
            var roleDescriptors = handler.GetIdentityProviderSingleSignOnDescriptor(metadata)
                .ToList();
            //ASSERT
            Assert.AreEqual(1, roleDescriptors.Count);
            Assert.AreEqual(1, roleDescriptors.Single().Roles.Count);
        }

        [Test]
        public void GetDelegateForIdpDescriptors_entity_descriptor_metadata_sp_role_Test()
        {
            //ARRANGE
            var metadata = EntityDescriptorProviderMock.GetSpEntityDescriptor("TestEntityId");
            var handler = new MetadataEntitityDescriptorHandler();
            //ACT
            var roleDescriptors = handler.GetRoleDescriptors<ServiceProviderSingleSignOnDescriptor>(metadata)
                .ToList();
            //ASSERT
            Assert.AreEqual(1, roleDescriptors.Count);
            Assert.AreEqual(1, roleDescriptors.Single().Roles.Count);
        }

        [Test]
        public void GetDelegateForIdpDescriptors_entities_descriptor_metadata_Test()
        {
            //ARRANGE
            var metadata = EntityDescriptorProviderMock.GetEntitiesDescriptor(1, 1);
            var handler = new MetadataEntitiesDescriptorHandler();
            //ACT
            var roleDescriptors = handler.GetRoleDescriptors<IdentityProviderSingleSignOnDescriptor>(metadata)
               .ToList();
            //ASSERT
            Assert.AreEqual(1, roleDescriptors.Count);
            Assert.AreEqual(1, roleDescriptors.Single().Roles.Count);
        }

        [Test]
        public void GetDelegateForIdpDescriptors_entities_descriptor_metadata_sp_role_Test()
        {
            //ARRANGE
            var metadata = EntityDescriptorProviderMock.GetEntitiesDescriptor(1, 1);
            var handler = new MetadataEntitiesDescriptorHandler();
            //ACT
            var roleDescriptors = handler.GetRoleDescriptors<ServiceProviderSingleSignOnDescriptor>(metadata)
               .ToList();
            //ASSERT
            Assert.AreEqual(1, roleDescriptors.Count);
            Assert.AreEqual(1, roleDescriptors.Single().Roles.Count);
        }

        [Test]
        public void GetDelegateForIdpDescriptors_entities_descriptor_metadata_multiple_child_entities_Test()
        {
            //ARRANGE
            var metadata = EntityDescriptorProviderMock.GetEntitiesDescriptor(2, 1);
            var handler = new MetadataEntitiesDescriptorHandler();
            //ACT
            var roleDescriptors = handler.GetRoleDescriptors<IdentityProviderSingleSignOnDescriptor>(metadata)
               .ToList();
            //ASSERT
            Assert.AreEqual(2, roleDescriptors.Count);
            Assert.AreEqual(1, roleDescriptors.ElementAt(0).Roles.Count);
            Assert.AreEqual(1, roleDescriptors.ElementAt(1).Roles.Count);
        }

        [Test]
        public void GetDelegateForIdpDescriptor_entities_descriptor_metadata_multiple_child_entities_Test()
        {
            //ARRANGE
            var metadata = EntityDescriptorProviderMock.GetEntitiesDescriptor(2, 1);
            var handler = new MetadataEntitiesDescriptorHandler();
            //ACT
            var roleDescriptors = handler.GetIdentityProviderSingleSignOnDescriptor(metadata)
               .ToList();
            //ASSERT
            Assert.AreEqual(2, roleDescriptors.Count);
            Assert.AreEqual(1, roleDescriptors.ElementAt(0).Roles.Count);
            Assert.AreEqual(1, roleDescriptors.ElementAt(1).Roles.Count);
        }
    }
}