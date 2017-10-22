using System;
using System.IdentityModel.Metadata;

namespace Shared.Federtion.Factories
{
    public interface IMetadataHandler
    {
        IdentityProviderSingleSignOnDescriptor GetIdentityProviderSingleSignOnDescriptor(MetadataBase metadata);
        Uri GetIdentityProviderSingleSignOnServices(IdentityProviderSingleSignOnDescriptor descriptor, Uri binding);
    }
}