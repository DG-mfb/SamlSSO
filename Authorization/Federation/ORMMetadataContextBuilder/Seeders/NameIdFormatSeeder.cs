using Kernel.Data.ORM;
using Kernel.Federation.Constants;
using ORMMetadataContextProvider.Models;

namespace ORMMetadataContextProvider.Seeders
{
    internal class NameIdFormatSeeder : Seeder
    {
        public override void Seed(IDbContext context)
        {
            var transientNameIdFormat = new NameIdFormat { Key = "Transient", Uri = NameIdentifierFormats.Transient };
            context.Add<NameIdFormat>(transientNameIdFormat);

            var persistentNameIdFormat = new NameIdFormat { Key = "Persistent", Uri = NameIdentifierFormats.Persistent };
            context.Add<NameIdFormat>(persistentNameIdFormat);

            Seeder._cache.Add(Seeder.NameIdKey, new[] { transientNameIdFormat, persistentNameIdFormat });
        }
    }
}