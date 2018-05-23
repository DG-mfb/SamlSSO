using System.IdentityModel.Metadata;
using Kernel.Federation.MetaData.Configuration.RoleDescriptors;

namespace WsFederationMetadataProvider.Metadata.DescriptorBuilders.SSODescriptorMemberBulders
{
    internal class OrganisationBuilder : RoleDescriptorMemberBuilder
    {
        protected override void BuildInternal(RoleDescriptor descriptor, RoleDescriptorConfiguration configuration)
        {
            if (configuration.Organisation == null)
                return;
            Organization organisation;
            if (SSODescriptorBuilderHelper.TryBuildOrganisation(configuration.Organisation, out organisation))
                descriptor.Organization = organisation;
        }
    }
}