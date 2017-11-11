using Kernel.Federation.MetaData;
using Shared.Federtion;

namespace WebClientMetadataWriter
{
    /// <summary>
    /// Writes metadata xml to a http stream
    /// </summary>
    internal class HttpMetadataWriter : MetadataWriter
    {
        protected override bool CanWrite(MetadataPublicationContext target)
        {
            return (target.MetadataPublishProtocol & MetadataPublicationProtocol.Http) == MetadataPublicationProtocol.Http;
        }
    }
}