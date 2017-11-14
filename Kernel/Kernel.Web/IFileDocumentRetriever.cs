using System;

namespace Kernel.Web
{
    public interface IFileDocumentRetriever : IDocumentRetriever
    {
        long MaxResponseContentBufferSize { get; set; }
    }
}