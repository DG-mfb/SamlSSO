﻿using System;
using System.IdentityModel.Metadata;
using System.Linq;
using Kernel.Federation.MetaData.Configuration.RoleDescriptors;

namespace WsFederationMetadataProvider.Metadata.DescriptorBuilders.SSODescriptorMemberBulders
{
    internal class NameIdentifierFormatsBuilder : RoleDescriptorMemberBuilder
    {
        protected override void BuildInternal(RoleDescriptor descriptor, RoleDescriptorConfiguration configuration)
        {
            var sSODescriptorConfiguration = configuration as SSODescriptorConfiguration;
            if (sSODescriptorConfiguration == null)
                throw new InvalidOperationException(String.Format("Configuration type expected: {0}.", typeof(SSODescriptorConfiguration).Name));

            if (sSODescriptorConfiguration.NameIdentifierFormats == null)
                throw new ArgumentNullException("singleLogoutServices");
            sSODescriptorConfiguration.NameIdentifierFormats.Aggregate(descriptor, (d, next) =>
            {
                ((SingleSignOnDescriptor)d).NameIdentifierFormats.Add(next);
                return d;
            });
        }
    }
}