using System;

namespace Kernel.Federation.MetaData
{
    [Flags]
    public enum MetadataPublicationProtocol
    {
        Memory= 0,
        Http = 1,
        FileSystem = 2
    }
}