using System;
using System.Collections.Generic;
using Kernel.Configuration;
using Kernel.Data.ORM;

namespace ORMMetadataContextProvider
{
    internal class DbCustomConfiguration : IDbCustomConfiguration
    {
        public DbCustomConfiguration()
        {
            this.Seeders = new List<ISeeder>();
            this.Schema = AppSettingsConfigurationManager.GetSetting("ssoSchema", "dbo");
        }
        public ICollection<ISeeder> Seeders { get; }

        public Func<IEnumerable<Type>> ModelsFactory { get; set; }

        public string Schema { get; }
    }
}