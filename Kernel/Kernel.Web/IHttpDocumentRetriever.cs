using System;

namespace Kernel.Web
{
    public interface IHttpDocumentRetriever : IDocumentRetriever
    {
        bool RequireHttps { get; set; }
        TimeSpan Timeout { get; set; }
        long MaxResponseContentBufferSize { get; set; }
    }
}