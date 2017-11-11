using System;
using System.IO;

namespace Kernel.Federation.MetaData
{
    public class MetadataPublicationContext
    {
        public MetadataPublicationContext(Stream targetStream, MetadataPublicationProtocol metadataPublishProtocol)
        {
            if (targetStream == null)
                throw new ArgumentNullException("targetStream");

            this.TargetStream = targetStream;
            this.MetadataPublishProtocol = metadataPublishProtocol;
        }
        public Stream TargetStream { get; }

        public MetadataPublicationProtocol MetadataPublishProtocol { get; }
    }
}