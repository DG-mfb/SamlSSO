using System;
using System.Collections.Generic;

namespace Kernel.Federation.MetaData
{
    public interface IMetadataHandler<TMetadata>
    {
        IEnumerable<EntityRoleDescriptor<TRole>> GetRoleDescriptors<TRole>(TMetadata metadata);
    }

    public class EntityRoleDescriptor<TRole>
    {
        public EntityRoleDescriptor(string entityId)
        {
            this.EntityId = entityId;
            this.Roles = new List<TRole>();
        }
        public string EntityId { get; }
        public ICollection<TRole> Roles { get; }
    }
}