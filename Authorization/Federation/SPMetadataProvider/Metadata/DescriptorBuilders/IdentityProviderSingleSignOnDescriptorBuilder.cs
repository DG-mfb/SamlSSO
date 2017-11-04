using System;
using System.IdentityModel.Metadata;
using System.Linq;
using Kernel.Federation.MetaData.Configuration.RoleDescriptors;

namespace WsFederationMetadataProvider.Metadata.DescriptorBuilders
{
    /// <summary>
    /// For testing perspose only
    /// </summary>
    internal class IdentityProviderSingleSignOnDescriptorBuilder : DescriptorBuilderBase<IdentityProviderSingleSignOnDescriptor>
    {
        protected override IdentityProviderSingleSignOnDescriptor BuildDescriptorInternal(RoleDescriptorConfiguration configuration)
        {
            var idpConfiguration = configuration as IdPSSODescriptorConfiguration;

            if (idpConfiguration == null)
                throw new InvalidCastException(string.Format("Expected type: {0} but was: {1}", typeof(IdPSSODescriptorConfiguration).Name, configuration.GetType().Name));

            var descriptor = new IdentityProviderSingleSignOnDescriptor
            {
                WantAuthenticationRequestsSigned = true
            };

            idpConfiguration.SignOnServices.Aggregate(descriptor, (d, next) =>
            {
                d.SingleSignOnServices.Add(new ProtocolEndpoint(next.Binding, next.Location));
                return d;
            });
            return descriptor;
        }
    }
}