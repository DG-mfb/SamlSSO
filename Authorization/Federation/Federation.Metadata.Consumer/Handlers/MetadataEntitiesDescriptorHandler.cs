using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using Kernel.Federation.MetaData;

namespace Federation.Metadata.FederationPartner.Handlers
{
    /// <summary>
    /// Handles multi entities metadata format
    /// </summary>
    internal class MetadataEntitiesDescriptorHandler : MetadataHandler, IMetadataHandler<EntitiesDescriptor>
    {
        public IEnumerable<EntityRoleDescriptor<TRole>> GetRoleDescriptors<TRole>(EntitiesDescriptor metadata)
        {
            return metadata.ChildEntities.SelectMany(x => x.RoleDescriptors.OfType<TRole>(), (d, r)=> new { d.EntityId.Id, r })
                .GroupBy(x => x.Id)
                .Select(x => x.Aggregate(new EntityRoleDescriptor<TRole>(x.Key), (d, next)=> { d.Roles.Add(next.r); return d; }));
        }
        
        protected override IEnumerable<EntityRoleDescriptor<TRole>> GetRoleDescriptors<TRole>(MetadataBase metadata)
        {
            return this.GetRoleDescriptors<TRole>((EntitiesDescriptor)metadata);
        }
    }
}