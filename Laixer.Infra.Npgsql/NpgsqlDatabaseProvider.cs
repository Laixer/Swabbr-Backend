using Laixer.Utility.Exceptions;
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
        public NpgsqlDatabaseProvider(IOptions<NpgsqlDatabaseProviderOptions> options)
        {
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            connectionString = options.Value.ConnectionString ?? throw new ConfigurationException("Missing Npgsql connection string");
        }

        /// <summary>
        /// Gets a connection scope.
        /// </summary>
        /// <returns><see cref="NpgsqlConnection"/></returns>
        public IDbConnection GetConnectionScope()
        {
            return new NpgsqlConnection(connectionString);
        }
    }

}
