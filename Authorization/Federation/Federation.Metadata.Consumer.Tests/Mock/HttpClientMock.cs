using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Federation.Metadata.Consumer.Tests.Mock
{
    internal class HttpClientMock : HttpClient
    {
        WebRequestHandler _messageHandler;
        public HttpClientMock(WebRequestHandler messageHandler)
        {
            this._messageHandler = messageHandler;
        }

        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("Content")));
            var message = new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
            return message;
        }
    }
}