using System;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace ECY.DataAccess
{
    public class DbConnectionFactory
    {
        private readonly DbProviderFactory provider;
        private readonly string connectionString;
        private readonly string name;

        public DbConnectionFactory(string connectionStringName)
        {
            if (connectionStringName == null) throw new ArgumentNullException("Connection string name");

            var conStr = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (conStr == null) throw new ConfigurationErrorsException(
                string.Format("Failed to find connection string named '{0}' in app.config or web.config.", connectionStringName));

            name = conStr.ProviderName;
            provider = DbProviderFactories.GetFactory(conStr.ProviderName);
            connectionString = conStr.ConnectionString;
        }

        public IDbConnection Create()
        {
            var connection = provider.CreateConnection();
            if (connection == null) throw new ConfigurationErrorsException( 
                string.Format("Failed to create a connection using the connection string named '{0}' in web.config or app.config", name));
            connection.ConnectionString = connectionString;
            return connection;
        }
    }
}
