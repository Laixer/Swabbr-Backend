using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Data;

namespace Laixer.Infra.Npgsql
{

    /// <summary>
    /// Used to provide connections to a postgresql database.
    /// </summary>
    public class NpgsqlDatabaseProvider : IDatabaseProvider
    {

        /// <summary>
        /// Contains our connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public NpgsqlDatabaseProvider(IOptions<NpgsqlDatabaseProviderOptions> options,
            IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            if (options.Value == null) { throw new ArgumentNullException(nameof(options.Value)); }
            if (string.IsNullOrEmpty(options.Value.ConnectionStringName)) { throw new InvalidOperationException(nameof(options.Value.ConnectionStringName)); }

            connectionString = configuration.GetConnectionString(options.Value.ConnectionStringName);
            if (string.IsNullOrEmpty(connectionString)) { throw new InvalidOperationException($"IConfiguration does not contains connection string with name {options.Value.ConnectionStringName}"); }
        }

        /// <summary>
        /// Gets a connection scope.
        /// </summary>
        /// <returns><see cref="NpgsqlConnection"/></returns>
        public IDbConnection GetConnectionScope() => new NpgsqlConnection(connectionString);

    }

}
