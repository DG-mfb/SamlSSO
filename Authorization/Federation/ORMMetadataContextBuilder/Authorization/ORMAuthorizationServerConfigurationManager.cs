using System;
using System.Threading;
using System.Threading.Tasks;
using Kernel.Cache;
using Kernel.Data.ORM;
using Kernel.Federation.Authorization;
using Shared.Federtion.Authorization;

namespace ORMMetadataContextProvider.Authorization
{
    internal class ORMAuthorizationServerConfigurationManager : AuthorizationServerConfigurationManager
    {
        private readonly IDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;

        public ORMAuthorizationServerConfigurationManager(IDbContext dbContext, ICacheProvider cacheProvider)
        {
            this._dbContext = dbContext;
            this._cacheProvider = cacheProvider;
        }

        public override Task<AuthorizationServerConfiguration> GetConfigurationAsync(string federationPartyId, CancellationToken cancel)
        {
            var configuration = new AuthorizationServerConfiguration
            {
                CreateToken = true,
                TokenResponseUrl = new Uri("http://localhost:61463/api/SSO")
            };
            return Task.FromResult(configuration);
        }
    }
}