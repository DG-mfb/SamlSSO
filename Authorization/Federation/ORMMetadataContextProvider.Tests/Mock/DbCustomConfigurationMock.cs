using System;
using System.Collections.Generic;
using Kernel.Data.ORM;

namespace ORMMetadataContextProvider.Tests.Mock
{
    internal class DbCustomConfigurationMock : IDbCustomConfiguration
    {
        public DbCustomConfigurationMock()
        {
            this.Seeders = new List<ISeeder>();
            this.Schema = "dbo";
        }
        public ICollection<ISeeder> Seeders { get; }

        public Func<IEnumerable<Type>> ModelsFactory { get; set; }
        public string Schema { get; }
    }
}