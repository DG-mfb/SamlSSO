using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using Kernel.Federation.MetaData;
using Shared.Federtion.Factories;

namespace Federation.Metadata.FederationPartner.Handlers
{
    internal abstract class MetadataHandler : IMetadataHandler
    {
        public IEnumerable<EntityRoleDescriptor<IdentityProviderSingleSignOnDescriptor>> GetIdentityProviderSingleSignOnDescriptor(MetadataBase metadata)
        {
            var descriptors = this.GetRoleDescriptors<IdentityProviderSingleSignOnDescriptor>(metadata);

            return descriptors;
        }

        public Uri GetIdentityProviderSingleSignOnServices(IdentityProviderSingleSignOnDescriptor descriptor, Uri binding)
        {
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            var endPoint = descriptor.SingleSignOnServices.FirstOrDefault(x => x.Binding == binding);
            if (endPoint == null)
                throw new InvalidOperationException(String.Format("No endpoint found for binding: {0}.", binding));
            return endPoint.Location;
        }

        protected abstract IEnumerable<EntityRoleDescriptor<TRole>> GetRoleDescriptors<TRole>(MetadataBase metadata);
    }
}