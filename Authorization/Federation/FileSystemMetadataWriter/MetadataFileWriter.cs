using Kernel.Federation.MetaData;
using Shared.Federtion;

namespace FileSystemMetadataWriter
{
    /// <summary>
    /// Writes metadata xml to a file stream
    /// </summary>
    internal class MetadataFileWriter : MetadataWriter
    {
        protected override bool CanWrite(MetadataPublicationContext target)
        {
            return (target.MetadataPublishProtocol & MetadataPublicationProtocol.FileSystem) == MetadataPublicationProtocol.FileSystem;
        }
    }
}