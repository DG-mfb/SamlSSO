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
        public IEnumerable<TRole> GetRoleDescriptors<TRole>(EntityDescriptor metadata)
        {
            return metadata.RoleDescriptors.OfType<TRole>();
        }
        
        protected override IEnumerable<TRole> GetRoleDescriptors<TRole>(MetadataBase metadata)
        {
            return this.GetRoleDescriptors<TRole>((EntityDescriptor)metadata);
        }
    }
}