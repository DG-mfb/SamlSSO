using System.IdentityModel.Metadata;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Federation.Metadata.Consumer.Tests.Mock
{
    internal class HttpClientMock : HttpClient
    {
        WebRequestHandler _messageHandler;
        public HttpClientMock(WebRequestHandler messageHandler)
        {
            this._messageHandler = messageHandler;
        }

        internal static string Metadata { get; set; }

        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var entity = EntityDescriptorProviderMock.GetIdpEntityDescriptor("IdpEntity");
            var serialiser = new MetadataSerializer();
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                serialiser.WriteMetadata(writer, entity);
                writer.Flush();
                HttpClientMock.Metadata = sb.ToString();
                var content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(HttpClientMock.Metadata)));
                var message = new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
                return message;
            }
        }
    }
}