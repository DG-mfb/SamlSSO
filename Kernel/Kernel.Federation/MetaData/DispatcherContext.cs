using System.Xml;

namespace Kernel.Federation.MetaData
{
    public class DispatcherContext
    {
        public DispatcherContext(XmlElement metadata, MetadataPublicationContext metadataPublishContext)
        {
            this.Metadata = metadata;
            this.MetadataPublishContext = metadataPublishContext;
        }
        public XmlElement Metadata { get; }
        public MetadataPublicationContext MetadataPublishContext { get; }
    }
}
