using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;
using Swabbr.Core.Exceptions;
using System;
using System.Data;

namespace Swabbr.Infrastructure.Providers
{
    /// <summary>
    ///     Used to provide connections to a postgresql database.
    /// </summary>
    public class NpgsqlDatabaseProvider : IDatabaseProvider
    {
        /// <summary>
        ///     Contains our connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public NpgsqlDatabaseProvider(IConfiguration configuration,
            IOptions<NpgsqlDatabaseProviderOptions> options)
        {
            var connectionStringName = options?.Value?.ConnectionStringName ?? throw new ConfigurationException("Missing Npgsql connection string name");

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            connectionString = configuration.GetConnectionString(connectionStringName) ?? throw new ConfigurationException("Misisng Npgsql connection string");
        }

        /// <summary>
        ///     Gets a connection scope.
        /// </summary>
        /// <returns>New npgsql connection.</returns>
        public IDbConnection GetConnectionScope() => new NpgsqlConnection(connectionString);
    }
}
