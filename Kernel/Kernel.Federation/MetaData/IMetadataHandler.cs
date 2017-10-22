using System;
using System.Collections.Generic;

namespace Kernel.Federation.MetaData
{
    public interface IMetadataHandler<TMetadata>
    {
        IEnumerable<TRole> GetRoleDescriptors<TRole>(TMetadata metadata);
    }
}