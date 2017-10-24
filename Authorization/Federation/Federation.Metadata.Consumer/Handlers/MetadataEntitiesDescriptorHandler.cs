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
        public IEnumerable<TRole> GetRoleDescriptors<TRole>(EntitiesDescriptor metadata)
        {
            return metadata.ChildEntities.SelectMany(x => x.RoleDescriptors.OfType<TRole>());
        }
        
        protected override IEnumerable<TRole> GetRoleDescriptors<TRole>(MetadataBase metadata)
        {
            return this.GetRoleDescriptors<TRole>((EntitiesDescriptor)metadata);
        }
    }
}