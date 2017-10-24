using Kernel.Federation.MetaData;
using Shared.Federtion;

namespace FileSystemMetadataWriter
{
    /// <summary>
    /// Writes metadata xml to a file stream
    /// </summary>
    internal class MetadataFileWriter : MetadataWriter
    {
        protected override bool CanWrite(MetadataPublishContext target)
        {
            return (target.MetadataPublishProtocol & MetadataPublishProtocol.FileSystem) == MetadataPublishProtocol.FileSystem;
        }
    }
}