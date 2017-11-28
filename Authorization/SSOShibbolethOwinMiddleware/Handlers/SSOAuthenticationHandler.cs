using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Federation.Protocols.Bindings.HttpPost;
using Federation.Protocols.Bindings.HttpRedirect;
using Kernel.DependancyResolver;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.MetaData;
using Kernel.Federation.MetaData.Configuration;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using Kernel.Federation.Protocols.Bindings.HttpRedirectBinding;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Shared.Federtion.Factories;

namespace SSOOwinMiddleware.Handlers
{
    internal class SSOAuthenticationHandler : AuthenticationHandler<SSOAuthenticationOptions>
    {
        private readonly ILogger _logger;
        private readonly IDependencyResolver _resolver;

        public SSOAuthenticationHandler(ILogger logger, IDependencyResolver resolver)
        {
            this._resolver = resolver;
            this._logger = logger;
        }

        public override Task<bool> InvokeAsync()
        {
            if (!this.Options.SSOPath.HasValue || base.Request.Path != this.Options.SSOPath)
                return base.InvokeAsync();
            Context.Authentication.Challenge(this.Options.AuthenticationType);
            return Task.FromResult(true);
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            if (!base.Options.AssertionEndpoinds.Contains(Request.Path))
                return null;

            try
            {
                this._logger.WriteInformation(String.Format("Authenticated response received to: {0}", Request.Path));
                if (string.Equals(this.Request.Method, "POST", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(this.Request.ContentType) && (this.Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) && this.Request.Body.CanRead))
                {
                    if (!this.Request.Body.CanSeek)
                    {
                        this._logger.WriteVerbose("Buffering request body");
                        MemoryStream memoryStream = new MemoryStream();
                        await this.Request.Body.CopyToAsync((Stream)memoryStream);
                        memoryStream.Seek(0L, SeekOrigin.Begin);
                        this.Request.Body = (Stream)memoryStream;
                    }

                    var form = await this.Request.ReadFormAsync();

                    var protocolFactory = this._resolver.Resolve<Func<string, IProtocolHandler>>();
                    var protocolHanlder = protocolFactory(Bindings.Http_Post);

                    var protocolContext = new SamlProtocolContext
                    {
                        ResponseContext = new HttpPostResponseContext
                        {
                            RequestUri = Request.Uri,
                            AuthenticationMethod = base.Options.AuthenticationType,
                            Form = form.ToDictionary(x => x.Key, v => form.Get(v.Key)) as IDictionary<string, string>
                        }
                    };
                    this._logger.WriteInformation(String.Format("Handle response entering."));
                    await protocolHanlder.HandleInbound(protocolContext);
                    var responseContext = protocolContext.ResponseContext as HttpPostResponseContext;
                    var identity = responseContext.Result;
                    if (identity != null)
                    {
                        this._logger.WriteInformation(String.Format("Authenticated. Authentication ticket issued."));
                        return new AuthenticationTicket(identity, new AuthenticationProperties());
                    }
                }
                this._logger.WriteInformation(String.Format("Authentication failed. No authentication ticket issued."));
                return null;
            }
            catch (Exception ex)
            {
                this._logger.WriteError(String.Format("An exceprion has been thrown when processing the response.", ex));
                return null;
            }
        }

        protected override async Task ApplyResponseChallengeAsync()
        {
            if (this.Response.StatusCode != 401)
                return;

            var challenge = this.Helper.LookupChallenge(this.Options.AuthenticationType, this.Options.AuthenticationMode);
            if (challenge == null)
                return;

            if (!this.Options.SSOPath.HasValue || base.Request.Path != this.Options.SSOPath)
                return;
            try
            {
                this._logger.WriteInformation(String.Format("Applying chanllenge for authenticationType: {0}, authenticationMode: {1}. Path: {2}", this.Options.AuthenticationType, this.Options.AuthenticationMode, this.Request.Path));
                var discoveryService = this._resolver.Resolve<IDiscoveryService<IOwinContext, string>>();
                   var federationPartyId = discoveryService.ResolveParnerId(Request.Context);
                
                var configurationManager = this._resolver.Resolve<IConfigurationManager<MetadataBase>>();
                var configuration = await configurationManager.GetConfigurationAsync(federationPartyId, new CancellationToken());
                
                if (configuration == null)
                    throw new InvalidOperationException("Cannot obtain metadata.");
                var metadataType = configuration.GetType();
                var handlerType = typeof(IMetadataHandler<>).MakeGenericType(metadataType);
                var handler = this._resolver.Resolve(handlerType) as IMetadataHandler;
                if (handler == null)
                    throw new InvalidOperationException(String.Format("Handler must implement: {0}", typeof(IMetadataHandler).Name));
                var idp = handler.GetIdentityProviderSingleSignOnDescriptor(configuration)
                    .Single();

                var federationPartyContextBuilder = this._resolver.Resolve<IAssertionPartyContextBuilder>();
                var federationContext = federationPartyContextBuilder.BuildContext(federationPartyId);

                var signInUrl = handler.GetIdentityProviderSingleSignOnServices(idp, federationContext.OutboundBinding);
               
                var requestContext = new AuthnRequestContext(signInUrl, base.Request.Uri, federationContext, idp.NameIdentifierFormats);
                var relayStateHandler = this._resolver.Resolve<IRelayStateHandler>();
                await relayStateHandler.BuildRelayState(requestContext);
                SamlOutboundContext outboundContext = null;
                if(federationContext.OutboundBinding == new Uri(Bindings.Http_Redirect))
                {
                    outboundContext = new HttpRedirectRequestContext
                    {
                        BindingContext = new HttpRedirectContext(requestContext),
                        DespatchDelegate = redirectUri =>
                        {
                            this.Response.Redirect(redirectUri.AbsoluteUri);
                            return Task.CompletedTask;
                        }
                    };
                }
                else
                {
                    outboundContext = new HttpPostRequestContext
                    {
                        BindingContext = new HttpPostContext(requestContext),
                        DespatchDelegate = (form) =>
                        {
                            Response.Write(form.ToString());
                            return Task.CompletedTask;
                        },
                    };
                }
                var protocolContext = new SamlProtocolContext
                {
                    RequestContext = outboundContext
                };
                var protocolFactory = this._resolver.Resolve<Func<string, IProtocolHandler>>();
                var protocolHanlder = protocolFactory(federationContext.OutboundBinding.AbsoluteUri);
                
                await protocolHanlder.HandleOutbound(protocolContext);
            }
            catch(Exception ex)
            {
                this._logger.WriteError("An exception has been thrown when applying challenge", ex);
                throw;
            }
        }
    }
}