using System;
using System.Net.Http;
using System.Net.Security;
using Kernel.DependancyResolver;
using Kernel.Security.Validation;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using SSOOwinMiddleware.Handlers;

namespace SSOOwinMiddleware
{
    internal class SSOOwinMiddleware : AuthenticationMiddleware<SSOAuthenticationOptions>
    {
        private readonly ILogger _logger;
        private readonly IDependencyResolver _resolver;
        public SSOOwinMiddleware(OwinMiddleware next, IAppBuilder app, SSOAuthenticationOptions options, IDependencyResolver resolver)
            : base(next, options)
        {
            this._resolver = resolver;
            this._logger = app.CreateLogger<SSOOwinMiddleware>();
            if (base.Options.BackchannelCertificateValidator == null)
            {
                base.Options.BackchannelCertificateValidator = this._resolver.Resolve<IBackchannelCertificateValidator>();
            }
 
            var httpClient = new HttpClient(SSOOwinMiddleware.ResolveHttpMessageHandler(this.Options))
            {
                Timeout = this.Options.BackchannelTimeout,
                MaxResponseContentBufferSize = 10485760L
            };

            this._resolver.RegisterFactory<Func<HttpClient>>(() =>
            {
                return () => httpClient;
            }, Lifetime.Transient);
        }
        
        protected override AuthenticationHandler<SSOAuthenticationOptions> CreateHandler()
        {
            return new SSOAuthenticationHandler(this._logger, this._resolver);
        }

        private static HttpMessageHandler ResolveHttpMessageHandler(SSOAuthenticationOptions options)
        {
            HttpMessageHandler httpMessageHandler = options.BackchannelHttpHandler ?? new WebRequestHandler();
            if (options.BackchannelCertificateValidator != null)
            {
                WebRequestHandler webRequestHandler = httpMessageHandler as WebRequestHandler;
                if (webRequestHandler == null)
                    throw new InvalidOperationException();
                webRequestHandler.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(options.BackchannelCertificateValidator.Validate);
            }
            return httpMessageHandler;
        }
    }
}