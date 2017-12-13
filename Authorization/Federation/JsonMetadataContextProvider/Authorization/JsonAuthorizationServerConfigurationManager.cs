using System;
using System.Threading;
using System.Threading.Tasks;
using Kernel.Cache;
using Kernel.Federation.Authorization;
using Shared.Federtion.Authorization;

namespace JsonMetadataContextProvider.Authorization
{
    internal class JsonAuthorizationServerConfigurationManager : AuthorizationServerConfigurationManager
    {
        private const string KeyPrefix = "auth";
        
        private readonly ICacheProvider _cacheProvider;

        public JsonAuthorizationServerConfigurationManager(ICacheProvider cacheProvider)
        {
            this._cacheProvider = cacheProvider;
        }

        public override Task<AuthorizationServerConfiguration> GetConfigurationAsync(string federationPartyId, CancellationToken cancel)
        {
            return Task.FromResult((AuthorizationServerConfiguration)null);
        }

        private static string FormatKey(string key)
        {
            return String.Format("{0}_{1}", JsonAuthorizationServerConfigurationManager.KeyPrefix, key);
        }
    }
}