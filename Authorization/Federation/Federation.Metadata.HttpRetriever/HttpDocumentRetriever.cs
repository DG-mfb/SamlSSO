using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using Kernel.Federation.FederationPartner;
using Kernel.Security.Validation;

namespace Federation.Metadata.HttpRetriever
{
    public class HttpDocumentRetriever : IDocumentRetriever
    {
        static HttpDocumentRetriever()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private readonly IBackchannelCertificateValidator _backchannelCertificateValidator;
        
        public bool RequireHttps { get; set; }
        
        public HttpDocumentRetriever(IBackchannelCertificateValidator backchannelCertificateValidator)
        {
            if (backchannelCertificateValidator == null)
                throw new ArgumentNullException("backchannelCertificateValidator");

            this._backchannelCertificateValidator = backchannelCertificateValidator;
        }

        public async Task<string> GetDocumentAsync(string address, CancellationToken cancel)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException("address");

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
                    var httpClient = new HttpClient(messageHandler)
                    {
                        Timeout = TimeSpan.FromSeconds(30),
                        MaxResponseContentBufferSize = 10485760L
                    };
                    using (httpClient)
                    {
                        var httpResponseMessage = await httpClient.GetAsync(address, cancel)
                            .ConfigureAwait(true);

                        var response = httpResponseMessage;
                        httpResponseMessage = null;
                        response.EnsureSuccessStatusCode();
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
    }
}