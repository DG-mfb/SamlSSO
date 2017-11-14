using System.Threading;
using System.Threading.Tasks;

namespace Kernel.Web
{
    public interface IDocumentRetriever
    {
        Task<string> GetDocumentAsync(string address, CancellationToken cancel);
    }
}