﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Kernel.Authorisation;
using Kernel.DependancyResolver;
using Kernel.Federation.Authorization;
using Kernel.Federation.FederationPartner;

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
            var federationPartyId = relayState["federationPartyId"].ToString();
            var configurationManager = this._resolver.Resolve<IConfigurationManager<AuthorizationServerConfiguration>>();
            var configuration = await configurationManager.GetConfigurationAsync(federationPartyId, CancellationToken.None);
            if (configuration == null)
                return;
            
            //if (!relayState.ContainsKey("returnUrl"))
            //    return;
            //var returnUrl = relayState["returnUrl"].ToString();
            var httpClient = new HttpClient();
            
            var result = await HttpClientExtensions.PostAsJsonAsync(httpClient, configuration.TokenResponseUrl, context.Token);
            
            context.RequestCompleted();
        }
    }
}