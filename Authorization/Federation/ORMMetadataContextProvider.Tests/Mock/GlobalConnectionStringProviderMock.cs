using System.Data.SqlClient;
using Kernel.Data.Connection;

namespace ORMMetadataContextProvider.Tests.Mock
{
    internal class GlobalConnectionStringProviderMock : IConnectionStringProvider<SqlConnectionStringBuilder>
    {
        public SqlConnectionStringBuilder GetConnectionString()
        {
            return new SqlConnectionStringBuilder
            {
                DataSource = "DG-MFB\\SQLEXPRESS_2016",
                InitialCatalog = "SSOConfiguration_Test",
                IntegratedSecurity = true
            };
        }
    }
}