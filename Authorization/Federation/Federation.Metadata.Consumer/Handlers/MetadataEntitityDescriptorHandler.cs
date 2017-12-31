using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using Kernel.Federation.MetaData;

namespace Federation.Metadata.FederationPartner.Handlers
{
    /// <summary>
    /// Handles single entity metadata format
    /// </summary>
    internal class MetadataEntitityDescriptorHandler : MetadataHandler, IMetadataHandler<EntityDescriptor>
    {
        public IEnumerable<EntityRoleDescriptor<TRole>> GetRoleDescriptors<TRole>(EntityDescriptor metadata)
        {
            return metadata.RoleDescriptors.OfType<TRole>()
                .Aggregate(new EntityRoleDescriptor<TRole>(metadata.EntityId.Id), (r, next) => { r.Roles.Add(next); return r; }, r => new[] { r });
        }
        
        protected override IEnumerable<EntityRoleDescriptor<TRole>> GetRoleDescriptors<TRole>(MetadataBase metadata)
        {
            return this.GetRoleDescriptors<TRole>((EntityDescriptor)metadata);
        }
    }
}