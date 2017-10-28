﻿using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using Provider.EntityFramework.Interseptors;

namespace Provider.EntityFramework.Configurtion
{
    public class CustomDbConfiguration : DbConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDbConfiguration" /> class, providing custom cache key for the
        /// model cache.
        /// </summary>
        public CustomDbConfiguration()
        {
            this.SetProviderServices("System.Data.SqlClient", SqlProviderServices.Instance);
            this.SetDefaultConnectionFactory(new SqlConnectionFactory());
            //this.SetModelCacheKey(new DbModelCacheKeyFactory().Create);
            this.AddInterceptor(new LoggingInterseptor());
        }
    }
}