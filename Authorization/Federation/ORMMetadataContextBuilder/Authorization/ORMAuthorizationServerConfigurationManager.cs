using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kernel.Cache;
using Kernel.Data.ORM;
using Kernel.Federation.Authorization;
using ORMMetadataContextProvider.Models.Autorisation;
using Shared.Federtion.Authorization;

namespace ORMMetadataContextProvider.Authorization
{
    internal class ORMAuthorizationServerConfigurationManager : AuthorizationServerConfigurationManager
    {
        private const string KeyPrefix = "auth";
        private readonly IDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;

        public ORMAuthorizationServerConfigurationManager(IDbContext dbContext, ICacheProvider cacheProvider)
        {
            this._dbContext = dbContext;
            this._cacheProvider = cacheProvider;
        }

        public override Task<AuthorizationServerConfiguration> GetConfigurationAsync(string federationPartyId, CancellationToken cancel)
        {
            AuthorizationServerConfiguration configuration = null;
            var key = ORMAuthorizationServerConfigurationManager.FormatKey(federationPartyId);
            if (this._cacheProvider.Contains(key))
            {
                configuration = this._cacheProvider.Get<AuthorizationServerConfiguration>(key);
            }
            else
            {
                var model = this._dbContext.Set<AuthorizationServerModel>()
                    .Select(x => new { x, x.FederationPartySettings.FederationPartyId })
                    .FirstOrDefault(x => x.FederationPartyId == federationPartyId);
                if (model != null)
                {
                    configuration = new AuthorizationServerConfiguration
                    {
                        CreateToken = model.x.UseTokenAuthorisation,
                        TokenResponseUrl = String.IsNullOrWhiteSpace(model.x.TokenResponseUrl) ? null : new Uri(model.x.TokenResponseUrl)
                    };

                    this._cacheProvider.Put(key, configuration);
                }
            }
            return Task.FromResult(configuration);
        }

        private static string FormatKey(string key)
        {
            return String.Format("{0}_{1}", ORMAuthorizationServerConfigurationManager.KeyPrefix, key);
        }
    }
}