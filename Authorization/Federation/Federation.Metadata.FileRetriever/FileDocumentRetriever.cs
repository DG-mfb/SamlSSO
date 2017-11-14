using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kernel.Web;

namespace Federation.Metadata.FileRetriever
{
    public class FileDocumentRetriever : IFileDocumentRetriever
    {
        public long MaxResponseContentBufferSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task<string> GetDocumentAsync(string address, CancellationToken cancel)
        {
            var request = WebRequest.Create(address);
            request.Method = "GET";
            using (var response = await request.GetResponseAsync())
            {
                using (var ms = new MemoryStream())
                {
                    await response.GetResponseStream().CopyToAsync(ms);
                    ms.Position = 0;
                    var content = Encoding.UTF8.GetString(ms.GetBuffer());
                    return content;
                }
            }
        }
    }
}