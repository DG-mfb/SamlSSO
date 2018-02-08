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
using Shared.Federtion.Forms;
using System;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SLOOwinMiddleware.Handlers
{
    internal class SLOAuthenticationHandler : AuthenticationHandler<SLOAuthenticationOptions>
    {
        private readonly ILogger _logger;
        private readonly IDependencyResolver _resolver;

        public SLOAuthenticationHandler(ILogger logger, IDependencyResolver resolver)
        {
            this._resolver = resolver;
            this._logger = logger;
        }

        public override Task<bool> InvokeAsync()
        {
            return base.InvokeAsync();
        }

        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            return Task.FromResult<AuthenticationTicket>(null);
        }

        protected async override Task ApplyResponseGrantAsync()
        {
            var signout = this.Helper.LookupSignOut(this.Options.AuthenticationType, this.Options.AuthenticationMode);
            if (signout == null)
                return;
            try
            {
                this._logger.WriteInformation(String.Format("Applying response grand for authenticationType: {0}, authenticationMode: {1}. Path: {2}", this.Options.AuthenticationType, this.Options.AuthenticationMode, this.Request.Path));
                var logoutContextBuilder = this._resolver.Resolve<ISamlLogoutContextResolver<IOwinRequest>>();
                var logoutContext = logoutContextBuilder.ResolveLogoutContext(Request);

                var federationPartyId = logoutContext.FederationPartyId;

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

                var signoutUrl = handler.GetIdentityProviderSingleLogoutService(idp, federationContext.OutboundBinding);

                var requestContext = new OwinLogoutRequestContext(Context, signoutUrl, base.Request.Uri, federationContext, logoutContext);
                var relayStateAppenders = this._resolver.ResolveAll<IRelayStateAppender>();
                foreach (var appender in relayStateAppenders)
                {
                    await appender.BuildRelayState(requestContext);
                }
                SamlOutboundContext outboundContext = null;
                if (federationContext.OutboundBinding == new Uri(Bindings.Http_Redirect))
                {
                    outboundContext = new HttpRedirectRequestContext
                    {
                        BindingContext = new RequestBindingContext(requestContext),
                        DespatchDelegate = redirectUri =>
                        {
                            this._logger.WriteInformation(String.Format("Redirecting to:\r\n{0}", redirectUri.AbsoluteUri));
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
                            this._logger.WriteInformation(String.Format("Writing saml form to the response."));
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
            catch (Exception ex)
            {
                this._logger.WriteError("An exception has been thrown when applying challenge", ex);
                throw;
            }
        }
    }
}