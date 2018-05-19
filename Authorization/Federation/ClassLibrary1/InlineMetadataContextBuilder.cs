﻿using System;
using System.IdentityModel.Tokens;
using System.Linq;
using Kernel.Federation.MetaData;
using Kernel.Federation.MetaData.Configuration;
using Kernel.Federation.MetaData.Configuration.Cryptography;
using Kernel.Federation.MetaData.Configuration.RoleDescriptors;

namespace InlineMetadataContextProvider
{
    internal class InlineMetadataContextBuilder : IInlineMetadataContextBuilder
    {
        private Func<MetadataGenerateRequest, bool> _includeOrganisationPredicate = _ => true;
        public Func<MetadataGenerateRequest, bool> IncludeOrganisationPredicate
        {
            private get
            {
                return this._includeOrganisationPredicate;
            }
            set
            {
                this._includeOrganisationPredicate = value;
            }
        }

        public MetadataContext BuildContext(MetadataGenerateRequest metadataGenerateContext)
        {
            var entityDescriptorConfiguration = MetadataHelper.BuildEntityDesriptorConfiguration(this._includeOrganisationPredicate(metadataGenerateContext));

            var keyDescriptorConfiguration = MetadataHelper.BuildKeyDescriptorConfiguration();

            RoleDescriptorConfiguration descriptorConfigurtion;
            switch(metadataGenerateContext.MetadataType)
            {
                case MetadataType.SP:
                    descriptorConfigurtion = MetadataHelper.BuildSPSSODescriptorConfiguration();
                    break;
                case MetadataType.Idp:
                    descriptorConfigurtion = MetadataHelper.BuildIdPSSODescriptorConfiguration();
                    break;
                default:
                    throw new System.Exception(string.Format("Unkown metadata type: {0}.", metadataGenerateContext.MetadataType));
            }
            
            entityDescriptorConfiguration.RoleDescriptors.Add(descriptorConfigurtion);
            
            var context = new MetadataContext
            {
                EntityDesriptorConfiguration = entityDescriptorConfiguration,
                SignMetadata = true
            };

            context.MetadataSigningContext = new MetadataSigningContext(SecurityAlgorithms.RsaSha1Signature, SecurityAlgorithms.Sha1Digest);

            context.MetadataSigningContext.KeyDescriptors.Add(keyDescriptorConfiguration.First(x => x.IsDefault && x.Use == KeyUsage.Signing));
            return context;
        }

        public void Dispose()
        {
        }
    }
}