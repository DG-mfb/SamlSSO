using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kernel.Authorisation;
using Kernel.DependancyResolver;
using Kernel.Federation.Authorization;
using Kernel.Federation.FederationPartner;
using Shared.Federtion.Constants;
using SSOOwinMiddleware.Contexts;

namespace WebApi.Token
{
    public class SSOAuthorizationServerProvider : IAuthorizationServerProvider
    {
        private readonly IDependencyResolver _resolver;
        public SSOAuthorizationServerProvider(IDependencyResolver resolver)
        {
            this._resolver = resolver;
        }
        async Task IAuthorizationServerProvider.TokenEndpointResponse<TContext>(TContext context)
        {
            var relayState = context.RelayState;
            if (!relayState.ContainsKey(RelayStateContstants.FederationPartyId))
                throw new InvalidOperationException("Federation party id is not in the relay state.");

            var federationPartyId = relayState[RelayStateContstants.FederationPartyId].ToString();
            var configurationManager = this._resolver.Resolve<IConfigurationManager<AuthorizationServerConfiguration>>();
            var configuration = await configurationManager.GetConfigurationAsync(federationPartyId, CancellationToken.None);
            
            //if no configuration for the parner return, no need to throw an exception.
            if (configuration == null)
                return;
            
            var sSOTokenEndpointResponseContext = context as SSOTokenEndpointResponseContext;
            if (sSOTokenEndpointResponseContext != null)
            {
                await sSOTokenEndpointResponseContext.Response.WriteAsync(context.Token);
                context.RequestCompleted();
            }
        }
    }
}