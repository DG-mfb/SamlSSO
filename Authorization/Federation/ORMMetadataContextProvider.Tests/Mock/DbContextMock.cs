using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Kernel.Data.ORM;
using Kernel.Reflection;
using ORMMetadataContextProvider.Models;
using ORMMetadataContextProvider.Seeders;

namespace ORMMetadataContextProvider.Tests.Mock
{
    internal class DbContextMock : IDbContext
    {
        private static int _initialised = 0;
        private static DbCustomConfigurationMock DbCustomConfiguration = new DbCustomConfigurationMock();
        private static Dictionary<string, FederationPartySettings> _settings = new Dictionary<string, FederationPartySettings>();
        public IDbCustomConfiguration CustomConfiguration { get { return DbContextMock.DbCustomConfiguration; } }

        static DbContextMock()
        {
            var seeders = ReflectionHelper.GetAllTypes(new[] { typeof(Seeder).Assembly }, t => !t.IsInterface && !t.IsAbstract && typeof(Seeder).IsAssignableFrom(t))
                .Select(x => Activator.CreateInstance(x) as Seeder);
            seeders.OrderBy(x => x.SeedingOrder).Aggregate(DbContextMock.DbCustomConfiguration.Seeders, (c, next) => { c.Add(next); return c; });
        }

        internal void Initialise()
        {
            if (Interlocked.CompareExchange(ref DbContextMock._initialised, 1, 0) == 1)
                return;
            foreach (var s in this.CustomConfiguration.Seeders)
                s.Seed(this);
        }
        public T Add<T>(T item) where T : class
        {
            var federationPartySettings = item as FederationPartySettings;
            if (federationPartySettings != null)
                DbContextMock._settings.Add(federationPartySettings.FederationPartyId, federationPartySettings);
            return item;
        }

        public void Dispose()
        {
            DbContextMock._settings.Clear();
        }

        public bool Remove<T>(T item) where T : class
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            return DbContextMock._settings.Count;
        }

        public IQueryable<T> Set<T>() where T : class
        {
            if(typeof(T) != typeof(FederationPartySettings))
                throw new NotSupportedException();
            return DbContextMock._settings.Select(x => x.Value).AsQueryable<FederationPartySettings>().Cast<T>();
        }
    }
}