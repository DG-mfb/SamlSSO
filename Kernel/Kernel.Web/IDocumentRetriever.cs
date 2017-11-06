using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kernel.Web
{
    public interface IDocumentRetriever
    {
        bool RequireHttps { get; set; }
        TimeSpan Timeout { get; set; }
        long MaxResponseContentBufferSize { get; set; }
        Task<string> GetDocumentAsync(string address, CancellationToken cancel);
    }
}