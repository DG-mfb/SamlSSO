using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;

namespace Shared.Federtion.Factories
{
    public interface IMetadataHandler
    {
        IEnumerable<IdentityProviderSingleSignOnDescriptor> GetIdentityProviderSingleSignOnDescriptor(MetadataBase metadata);
        Uri GetIdentityProviderSingleSignOnServices(IdentityProviderSingleSignOnDescriptor descriptor, Uri binding);
    }
}