using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.Threading;
using System.Threading.Tasks;
using Kernel.Federation.FederationPartner;

namespace Federation.Protocols.Test.Mock.Metadata
{
    internal class ConfigurationRetrieverMock : IConfigurationRetriever<MetadataBase>
    {
        public Action<MetadataBase> MetadataReceivedCallback { get; set; }

        public Task<MetadataBase> GetAsync(FederationPartyConfiguration context, CancellationToken cancel)
        {
            var metadata = this.GetMetadata();
            return Task.FromResult(metadata);
        }

        private MetadataBase GetMetadata()
        {
            var entity = new EntityDescriptor(new EntityId("local"));
            var idp = new IdentityProviderSingleSignOnDescriptor();
            var certificate = AssertionFactroryMock.GetMockCertificate();
            var ski = new SecurityKeyIdentifier(new X509RawDataKeyIdentifierClause(certificate));
            idp.Keys.Add(new KeyDescriptor(ski) { Use = KeyType.Signing});
            entity.RoleDescriptors.Add(idp);
            return entity;
        }
    }
}