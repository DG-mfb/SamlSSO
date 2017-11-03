using System;
using System.IdentityModel.Metadata;
using Kernel.Federation.MetaData.Configuration.RoleDescriptors;
using Shared.Federtion.Constants;

namespace WsFederationMetadataProvider.Metadata.DescriptorBuilders
{
    /// <summary>
    /// For testing perspose only
    /// </summary>
    internal class IdentityProviderSingleSignOnDescriptorBuilder : DescriptorBuilderBase<IdentityProviderSingleSignOnDescriptor>
    {
        protected override IdentityProviderSingleSignOnDescriptor BuildDescriptorInternal(RoleDescriptorConfiguration configuration)
        {
            var spConfiguration = configuration as SPSSODescriptorConfiguration;

            if (spConfiguration == null)
                throw new InvalidCastException(string.Format("Expected type: {0} but was: {1}", typeof(SPSSODescriptorConfiguration).Name, configuration.GetType().Name));

            var descriptor = new IdentityProviderSingleSignOnDescriptor
            {
                WantAuthenticationRequestsSigned = true
            };

            descriptor.SingleSignOnServices.Add(new ProtocolEndpoint(new Uri(ProtocolBindings.HttpRedirect), new Uri("http://localhost:63337/sso/login.aspx")));
            return descriptor;
        }
    }
}