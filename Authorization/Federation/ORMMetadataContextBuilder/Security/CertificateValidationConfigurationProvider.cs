﻿using System;
using System.Linq;
using Kernel.Cache;
using Kernel.Data.ORM;
using Kernel.Security.Configuration;
using Kernel.Security.Validation;
using ORMMetadataContextProvider.Models;

namespace ORMMetadataContextProvider.Security
{
    internal class CertificateValidationConfigurationProvider : ICertificateValidationConfigurationProvider
    {
        private const string PinsKey = "{0}_backchannel";
        private readonly IDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;

        public CertificateValidationConfigurationProvider(IDbContext dbContext, ICacheProvider cacheProvider)
        {
            this._dbContext = dbContext;
            this._cacheProvider = cacheProvider;
        }

        public BackchannelConfiguration GeBackchannelConfiguration(string federationPartyId)
        {
            var key = String.Format(CertificateValidationConfigurationProvider.PinsKey, federationPartyId.ToLower());
            if (this._cacheProvider.Contains(key))
                return this._cacheProvider.Get<BackchannelConfiguration>(key);

            var settings = this._dbContext.Set<FederationPartySettings>()
                .Where(x => x.FederationPartyId == federationPartyId)
                .Select(r => new { r.SecuritySettings, Pins = r.CertificatePins.Select(p => new { p.PinType, p.Value, p.Algorithm }) })
                .FirstOrDefault();
            if (settings is null)
                throw new InvalidOperationException(String.Format("No federationParty configuration found for federationPartyId: {0}", federationPartyId));

            var configuration = new BackchannelConfiguration
            {
                UsePinningValidation = settings.SecuritySettings.PinnedValidation,
                BackchannelValidatorResolver = new Kernel.Data.TypeDescriptor(settings.SecuritySettings.PinnedTypeValidator)
            };

            configuration.Pins = settings.Pins.GroupBy(k => k.PinType, v => v.Value)
                .ToDictionary(k => k.Key.ToString(), v => v.Select(r => r));
            this._cacheProvider.Put(key, configuration);
            return configuration;
        }

        public CertificateValidationConfiguration GetConfiguration(string federationPartyId)
        {
            var settings = this._dbContext.Set<FederationPartySettings>()
                .Where(x => x.FederationPartyId == federationPartyId)
                .Select(r => new { r.SecuritySettings, Pins = r.CertificatePins.Select(p => new { p.PinType, p.Value, p.Algorithm }) })
                .FirstOrDefault();

            if (settings is null)
                throw new InvalidOperationException(String.Format("No federationParty configuration found for federationPartyId: {0}", federationPartyId));

            var configuration = new CertificateValidationConfiguration
            {
                X509CertificateValidationMode = settings.SecuritySettings.X509CertificateValidationMode,
            };
            
            var rules = settings.SecuritySettings.CertificateValidationRules.Where(x => x.Type != null)
                .ToList();
            rules.Aggregate(configuration.ValidationRules, (t, next) =>
            {
                t.Add(new ValidationRuleDescriptor(next.Type));
                return t;
            });
            return configuration;
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            if (this._dbContext != null)
                this._dbContext.Dispose();
        }
    }
}