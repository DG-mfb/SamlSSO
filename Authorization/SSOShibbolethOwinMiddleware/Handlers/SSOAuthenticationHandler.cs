using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Federation.Protocols.Bindings.HttpPost;
using Federation.Protocols.Bindings.HttpRedirect;
using Kernel.Authorisation;
using Kernel.DependancyResolver;
using Kernel.Federation.Authorization;
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
using Shared.Federtion.Constants;
using Shared.Federtion.Factories;
using Shared.Federtion.Forms;
using SSOOwinMiddleware.Contexts;

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

        public override async Task<bool> InvokeAsync()
        {
            if (!this.Options.SSOPath.HasValue || base.Request.Path != this.Options.SSOPath)
            {
                var ticket = await base.AuthenticateAsync();
                if (ticket != null)
                {
                    var relayState = new Dictionary<string, object> { { RelayStateContstants.FederationPartyId, ticket.Properties.Dictionary[RelayStateContstants.FederationPartyId] } };
                    AuthenticationTokenCreateContext context;
                    var tokenCreated = this.TryCreateToken(ticket, relayState, out context);
                    if (tokenCreated && !String.IsNullOrWhiteSpace(context.Token))
                    {
                        var complete = await this.TryTokenEndpointResponse(context, relayState);

                        return complete;
                    }
                }
                return await base.InvokeAsync();
            }
            else
            {
                Context.Authentication.Challenge(this.Options.AuthenticationType);
                return true;
            }
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            if (base.Options.AssertionEndpoinds.Count > 0 && !base.Options.AssertionEndpoinds.Contains(Request.Path))
                return null;

            try
            {
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
                    if (form.Get(HttpRedirectBindingConstants.SamlResponse) == null)
                        return null;

                    this._logger.WriteInformation(String.Format("Saml2 response received"));
                    var protocolFactory = this._resolver.Resolve<Func<string, IProtocolHandler>>();
                    var protocolHanlder = protocolFactory(Bindings.Http_Post);

                    var protocolContext = new SamlProtocolContext
                    {
                        ResponseContext = new HttpPostInboundContext
                        {
                            RequestUri = Request.Uri,
                            AuthenticationMethod = base.Options.AuthenticationType,
                            Form = form.ToDictionary(x => x.Key, v => form.Get(v.Key)) as IDictionary<string, string>
                        }
                    };
                    this._logger.WriteInformation(String.Format("Handle response entering."));
                    await protocolHanlder.HandleInbound(protocolContext);
                    var responseContext = protocolContext.ResponseContext as HttpPostInboundContext;
                    var identity = responseContext.Identity;
                    if (identity != null)
                    {
                        this._logger.WriteInformation(String.Format("Authenticated. Authentication ticket issued."));
                        var properties = new AuthenticationProperties();
                        properties.Dictionary.Add(RelayStateContstants.FederationPartyId, ((IDictionary<string, object>)responseContext.RelayState)[RelayStateContstants.FederationPartyId].ToString());
                        var ticket = new AuthenticationTicket(identity, properties);
                        return ticket;
                    }
                    this._logger.WriteInformation(String.Format("Authentication failed. No authentication ticket issued."));
                }
                
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
                    .Single().Roles.Single();

                var federationPartyContextBuilder = this._resolver.Resolve<IAssertionPartyContextBuilder>();
                var federationContext = federationPartyContextBuilder.BuildContext(federationPartyId);

                var signInUrl = handler.GetIdentityProviderSingleSignOnServices(idp, federationContext.OutboundBinding);
               
                var requestContext = new OwinAuthnRequestContext(Context, signInUrl, base.Request.Uri, federationContext, idp.NameIdentifierFormats);
                var relayStateAppenders = this._resolver.ResolveAll<IRelayStateAppender>();
                foreach (var appender in relayStateAppenders)
                {
                    await appender.BuildRelayState(requestContext);
                }
                SamlOutboundContext outboundContext = null;
                if(federationContext.OutboundBinding == new Uri(Bindings.Http_Redirect))
                {
                    outboundContext = new HttpRedirectRequestContext
                    {
                        BindingContext = new RequestBindingContext(requestContext),
                        DespatchDelegate = redirectUri =>
                        {
                            this.Response.Redirect(redirectUri.AbsoluteUri);
                            return Task.CompletedTask;
                        }
                    };
                }
                else
                {
                    outboundContext = new HttpPostRequestContext(new SAMLForm())
                    {
                        BindingContext = new RequestPostBindingContext(requestContext),
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

        private async Task<bool> TryTokenEndpointResponse(AuthenticationTokenCreateContext context, IDictionary<string, object> relayState)
        {
            IAuthorizationServerProvider authorizationServerProvider;
            if (!this._resolver.TryResolve<IAuthorizationServerProvider>(out authorizationServerProvider))
                return false;
            var sSOTokenEndpointResponseContext = new SSOTokenEndpointResponseContext(base.Context, base.Options, context.Token, relayState);
            await authorizationServerProvider.TokenEndpointResponse(sSOTokenEndpointResponseContext);
            return sSOTokenEndpointResponseContext.IsRequestCompleted;
        }

        private bool TryCreateToken(AuthenticationTicket ticket, IDictionary<string, object> relayState, out AuthenticationTokenCreateContext context)
        {
            context = null;
            if (!relayState.ContainsKey(RelayStateContstants.FederationPartyId))
                throw new InvalidOperationException("Federation party id is not in the relay state.");

            var federationPartyId = relayState[RelayStateContstants.FederationPartyId].ToString();
            var configurationManager = this._resolver.Resolve<IConfigurationManager<AuthorizationServerConfiguration>>();
            var configurationTask = configurationManager.GetConfigurationAsync(federationPartyId, CancellationToken.None);
            configurationTask.Wait();
            var configuration = configurationTask.Result;

            //if no configuration for the parner return, no need to throw an exception.
            if (configuration == null || !configuration.CreateToken)
                return false;
            ISecureDataFormat<AuthenticationTicket> dataFormat;
            if (!this._resolver.TryResolve<ISecureDataFormat<AuthenticationTicket>>(out dataFormat))
                return false;
            context = new AuthenticationTokenCreateContext(base.Context, dataFormat, ticket);
            IAuthenticationTokenProvider authenticationTokenProvider;
            if (!this._resolver.TryResolve<IAuthenticationTokenProvider>(out authenticationTokenProvider))
                return false;
            authenticationTokenProvider.Create(context);
            return true;
        }
    }
}