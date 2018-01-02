using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using Kernel.Federation.MetaData;

namespace Shared.Federtion.Factories
{
    public interface IMetadataHandler
    {
        IEnumerable<EntityRoleDescriptor<IdentityProviderSingleSignOnDescriptor>> GetIdentityProviderSingleSignOnDescriptor(MetadataBase metadata);
        Uri GetIdentityProviderSingleSignOnServices(IdentityProviderSingleSignOnDescriptor descriptor, Uri binding);
        Uri GetIdentityProviderSingleLogoutService(IdentityProviderSingleSignOnDescriptor descriptor, Uri binding);
    }
}