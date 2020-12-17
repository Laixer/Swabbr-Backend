using Microsoft.Extensions.Options;
using System;
using System.Data.Common;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Providers
{
    /// <summary>
    ///     Abstract database provider class, used
    ///     for managing connections and commands.
    /// </summary>
    internal abstract class DatabaseProvider
    {
        /// <summary>
        ///     Options field which is accessible to all 
        ///     implementations of this abstract class.
        /// </summary>
        protected readonly DatabaseProviderOptions _options;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public DatabaseProvider(IOptions<DatabaseProviderOptions> options)
            => _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        /// <summary>
        ///     Gets or creates a connection scope 
        ///     to the database.
        /// </summary>
        /// <returns>The connection scope.</returns>
        public abstract DbConnection CreateConnectionScope();

        /// <summary>
        ///     Open database connection.
        /// </summary>
        /// <param name="token">The cancellation instruction.</param>
        /// <returns>See <see cref="DbConnection"/>.</returns>
        public virtual async Task<DbConnection> OpenConnectionScopeAsync(CancellationToken token = default)
        {
            var connection = CreateConnectionScope();
            await connection.OpenAsync(token);
            return connection;
        }

        /// <summary>
        ///     Create command on the database connection.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">Database connection.</param>
        /// <returns>New database command.</returns>
        public virtual DbCommand CreateCommand(string cmdText, DbConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            var cmd = connection.CreateCommand();
            cmd.CommandText = cmdText;
            return cmd;
        }

        /// <summary>
        ///     Handle database exception.
        /// </summary>
        /// <param name="edi">Captured exception.</param>
        internal virtual void HandleException(ExceptionDispatchInfo edi) 
            => edi.Throw();
    }
}
