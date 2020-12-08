using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;
using Swabbr.Core.Types;
using Swabbr.Core.Exceptions;
using System;
using System.Data.Common;
using System.Runtime.ExceptionServices;

namespace Swabbr.Infrastructure.Providers
{
    /// <summary>
    ///     Used to provide connections to a postgresql database.
    /// </summary>
    internal class NpgsqlDatabaseProvider : DatabaseProvider
    {
        /// <summary>
        ///     Contains our connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public NpgsqlDatabaseProvider(IConfiguration configuration, IOptions<DatabaseProviderOptions> options)
            : base(options)
        {
            var connectionStringName = options?.Value?.ConnectionStringName ?? throw new ConfigurationException("Missing Npgsql connection string name");

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            connectionString = configuration.GetConnectionString(connectionStringName) ?? throw new ConfigurationException("Misisng Npgsql connection string");
        }

        /// <summary>
        ///     Static constructor which will be called once.
        /// </summary>
        static NpgsqlDatabaseProvider()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<FollowMode>("application.follow_mode");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<FollowRequestStatus>("application.follow_request_status");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Gender>("application.gender");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<PushNotificationPlatform>("application.push_notification_platform");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<ReactionStatus>("entities.reaction_status");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<VlogStatus>("entities.vlog_status");
        }

        /// <summary>
        ///     Creates a new postgresql connection.
        /// </summary>
        /// <returns>Postgresql connection scope.</returns>
        public override DbConnection CreateConnectionScope()
            => new NpgsqlConnection(connectionString);

        /// <summary>
        ///     Creates a new npgsql database command.
        /// </summary>
        /// <param name="cmdText">The sql statement.</param>
        /// <param name="connection">the connection scope.</param>
        /// <returns>Database command object.</returns>
        public override DbCommand CreateCommand(string cmdText, DbConnection connection)
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            => new NpgsqlCommand(cmdText, connection as NpgsqlConnection);
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities

        // TODO Look into this.
        /// <summary>
        ///     Handles a thrown database exception.
        /// </summary>
        internal override void HandleException(ExceptionDispatchInfo edi)
        {
            if (edi.SourceException is PostgresException exception)
            {
                switch (exception.SqlState)
                {
                    case Npgsql.PostgresErrorCodes.ForeignKeyViolation:
                        throw new ReferencedEntityNotFoundException(exception.Message, exception);
                }
            }

            base.HandleException(edi);
        }
    }
}
