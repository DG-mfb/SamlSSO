using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using Kernel.Configuration;
using Kernel.Security.Validation;
using Kernel.Web;

namespace Federation.Metadata.HttpRetriever
{
    /// <summary>
    /// Retrieve metadata document from given Url
    /// </summary>
    public class HttpDocumentRetriever : IHttpDocumentRetriever
    {
        static HttpDocumentRetriever()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private readonly IBackchannelCertificateValidator _backchannelCertificateValidator;
        
        public bool RequireHttps { get; set; }
        public TimeSpan Timeout { get; set; }
        public long MaxResponseContentBufferSize { get; set; }
        public ICustomConfigurator<IHttpDocumentRetriever> HttpDocumentRetrieverConfigurator { private get; set; }

        /// <summary>
        /// Initialise an instance of Http document retriever
        /// </summary>
        /// <param name="backchannelCertificateValidator"></param>
        public HttpDocumentRetriever(IBackchannelCertificateValidator backchannelCertificateValidator)
        {
            if (backchannelCertificateValidator == null)
                throw new ArgumentNullException("backchannelCertificateValidator");

            this._backchannelCertificateValidator = backchannelCertificateValidator;
            this.Timeout = TimeSpan.FromSeconds(30);
            this.MaxResponseContentBufferSize = 10485760L;
            this.RequireHttps = true;
        }

        /// <summary>
        /// Retrieve a detadata document from the web
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public async Task<string> GetDocumentAsync(string address, CancellationToken cancel)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException("address");

            if (this.HttpDocumentRetrieverConfigurator != null)
            {
                this.HttpDocumentRetrieverConfigurator.Configure(this);
            }

            if (this.RequireHttps && !Utility.IsHttps(address))
                throw new ArgumentException(string.Format("IDX10108: The address specified '{0}' is not valid as per HTTPS scheme. Please specify an https address for security reasons. If you want to test with http address, set the RequireHttps property  on IDocumentRetriever to false.", (object)address), "address");
            
            string str1;
            try
            {
                var messageHandler = new WebRequestHandler
                {
                    ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(this._backchannelCertificateValidator.Validate)
                };
                using (messageHandler)
                {
                    var httpClient = this.GetHttpClient(messageHandler);
                    using (httpClient)
                    {
                        var httpResponseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, address), cancel)
                           .ConfigureAwait(true);
                        var response = httpResponseMessage;
                        httpResponseMessage = null;
                        if (response == null)
                            throw new ArgumentNullException(nameof(response));
                        response.EnsureSuccessStatusCode();
                        if (response.Content == null)
                            throw new ArgumentNullException(nameof(response.Content));
                        var str = await response.Content.ReadAsStringAsync()
                            .ConfigureAwait(true);
                        str1 = str;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IOException(String.Format("IDX10804: Unable to retrieve document from: '{0}'.", address), ex);
            }
            return str1;
        }

        protected virtual HttpClient GetHttpClient(WebRequestHandler messageHandler)
        {
            return new HttpClient(messageHandler)
            {
                Timeout = this.Timeout,
                MaxResponseContentBufferSize = this.MaxResponseContentBufferSize
            };
        }
    }
}