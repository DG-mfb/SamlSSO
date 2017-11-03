using System;
using System.IdentityModel.Metadata;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.MetaData;
using Kernel.Logging;
using Kernel.Security.CertificateManagement;

namespace WsFederationMetadataProvider.Metadata
{
    public class IdpSSOMetadataProvider : MetadataGeneratorBase, IIdPMetadataGenerator
    {
        public IdpSSOMetadataProvider(IFederationMetadataDispatcher metadataDispatcher, ICertificateManager certificateManager, IMetadataSerialiser<MetadataBase> serialiser, Func<MetadataGenerateRequest, FederationPartyConfiguration> configuration, ILogProvider logProvider)
            :base(metadataDispatcher, certificateManager, serialiser, configuration, logProvider)
        { }
    }
}