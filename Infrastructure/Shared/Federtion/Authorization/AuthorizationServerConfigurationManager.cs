using System;
using System.Threading;
using System.Threading.Tasks;
using Kernel.Federation.Authorization;
using Kernel.Federation.FederationPartner;

namespace Shared.Federtion.Authorization
{
    public abstract class AuthorizationServerConfigurationManager : IConfigurationManager<AuthorizationServerConfiguration>
    {
        public abstract Task<AuthorizationServerConfiguration> GetConfigurationAsync(string federationPartyId, CancellationToken cancel);
        
        public virtual void RequestRefresh(string federationPartyId)
        {
            throw new NotImplementedException();
        }
    }
}