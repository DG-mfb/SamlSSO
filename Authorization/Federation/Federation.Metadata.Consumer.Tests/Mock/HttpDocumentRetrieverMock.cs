using System.Net.Http;
using Federation.Metadata.HttpRetriever;
using Kernel.Security.Validation;

namespace Federation.Metadata.Consumer.Tests.Mock
{
    internal class HttpDocumentRetrieverMock : HttpDocumentRetriever
    {
        public HttpDocumentRetrieverMock(IBackchannelCertificateValidator backchannelCertificateValidator) : base(backchannelCertificateValidator)
        {
        }

        protected override HttpClient GetHttpClient(WebRequestHandler messageHandler)
        {
            return new HttpClientMock(messageHandler);
        }
    }
}