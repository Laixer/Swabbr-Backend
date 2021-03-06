﻿using Swabbr.Core.Exceptions;
using Swabbr.Infrastructure.Providers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Database
{
    /// <summary>
    ///     Database context.
    /// </summary>
    internal class DatabaseContext : IAsyncDisposable
    {
        /// <summary>
        ///     Data provider interface.
        /// </summary>
        public DatabaseProvider DatabaseProvider { get; set; }

        /// <summary>
        ///     Application context.
        /// </summary>
        public Core.AppContext AppContext { get; set; }

        /// <summary>
        ///     Database connection.
        /// </summary>
        public DbConnection Connection { get; private set; }

        /// <summary>
        ///     Database command.
        /// </summary>
        public DbCommand Command { get; private set; }

        /// <summary>
        ///     Encapsulated database reader.
        /// </summary>
        /// <remarks>
        ///     Accessed through <see cref="Reader"/>.
        /// </remarks>
        private DbDataReader reader;

        /// <summary>
        ///     Database reader.
        /// </summary>
        /// <remarks>
        ///     Setting this property will gracefully
        ///     dispose the existing reader if it exists.
        /// </remarks>
        public DbDataReader Reader
        {
            get => reader;
            set
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
                reader = value;
            }
        }

        // FUTURE: Log debug
        /// <summary>
        ///     Dispatch the exception to the databse provider.
        /// </summary>
        /// <remarks>
        ///     This method should not be called directly but only in the context
        ///     of a caught exception. The exception block should still re-throw
        ///     the exception since this handler *cannot* guarantee end of execution.
        ///     <para>
        ///         The exception is captured first before its send to a remote
        ///         handler. The <see cref="ExceptionDispatchInfo"/> preserves
        ///         the stacktrace and other location properties when the exception
        ///         is re-thrown.
        ///     </para>
        /// </remarks>
        /// <param name="edi">Captured exception.</param>
        private void HandleException(ExceptionDispatchInfo edi) =>
            DatabaseProvider.HandleException(edi);

        /// <summary>
        ///     Initialize database context.
        /// </summary>
        /// <param name="cmdText">The text of the sql query.</param>
        public async ValueTask InitializeAsync(string cmdText)
        {
            Connection = await DatabaseProvider.OpenConnectionScopeAsync(AppContext.CancellationToken);
            Command = DatabaseProvider.CreateCommand(cmdText, Connection);
        }

        /// <summary>
        ///     Add parameter with key and value to command.
        /// </summary>
        /// <remarks>
        ///     Sets a database null value if the object value is null.
        /// </remarks>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        public void AddParameterWithValue(string parameterName, object value)
        {
            var parameter = Command.CreateParameter();

            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;

            if (value is string && string.IsNullOrEmpty(value as string))
            {
                parameter.Value = DBNull.Value;
            }

            Command.Parameters.Add(parameter);
        }

        /// <summary>
        ///     Add parameter with key and value to command. If the
        ///     parameter already exists, overwrite it.
        /// </summary>
        /// <remarks>
        ///     Sets a database null value if the object value is null.
        /// </remarks>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        public void AddOrOverwriteParameterWithValue(string parameterName, object value)
        {
            var parameters = Command.Parameters;
            if (parameters.Contains(parameterName))
            {
                var index = parameters.IndexOf(parameterName);
                parameters.RemoveAt(index);
            }

            AddParameterWithValue(parameterName, value);
        }

        // FUTURE: Do not depend on Npgsql. Too npgsql specific.
        /// <summary>
        ///     Add parameter with key and json value to command.
        /// </summary>
        /// <remarks>
        ///     Sets a database null value if the object value is null.
        /// </remarks>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        public void AddJsonParameterWithValue(string parameterName, object value)
        {
            var parameter = new Npgsql.NpgsqlParameter(parameterName, NpgsqlTypes.NpgsqlDbType.Jsonb)
            {
                Value = value ?? DBNull.Value
            };

            Command.Parameters.Add(parameter);
        }

        /// <summary>
        ///     Execute command and return a reader.
        /// </summary>
        /// <param name="readAhead">Read to first row.</param>
        /// <param name="hasRowsGuard">Throw if no rows returned.</param>
        public async Task<DbDataReader> ReaderAsync(bool readAhead = true, bool hasRowsGuard = true)
        {
            try
            {
                Reader = await Command.ExecuteReaderAsync(AppContext.CancellationToken);
                if (!Reader.HasRows && hasRowsGuard)
                {
                    throw new EntityNotFoundException();
                }

                if (readAhead)
                {
                    await Reader.ReadAsync(AppContext.CancellationToken);
                }

                return Reader;
            }
            catch (DbException exception)
            {
                HandleException(ExceptionDispatchInfo.Capture(exception));
                throw;
            }
        }

        /// <summary>
        ///     Execute command and return a reader per row.
        /// </summary>
        /// <param name="hasRowsGuard">Throw if no rows returned.</param>
        public async IAsyncEnumerable<DbDataReader> EnumerableReaderAsync(bool hasRowsGuard = false)
        {
            try
            {
                Reader = await Command.ExecuteReaderAsync(AppContext.CancellationToken);
                if (!Reader.HasRows && hasRowsGuard)
                {
                    throw new EntityNotFoundException();
                }
            }
            catch (DbException exception)
            {
                HandleException(ExceptionDispatchInfo.Capture(exception));
                throw;
            }

            // NOTE: An unfortunate consequence of the yield return is the incapability
            //       of running inside a try-catch block. It should be rare for an exception
            //       to occur after the command has been executed, but not impossible.
            while (await Reader.ReadAsync(AppContext.CancellationToken))
            {
                yield return Reader;
            }
        }

        /// <summary>
        ///     Execute command.
        /// </summary>
        /// <param name="hasRowGuard">Throw if no rows were affected.</param>
        public async Task NonQueryAsync(bool hasRowGuard = true)
        {
            try
            {
                var affected = await Command.ExecuteNonQueryAsync(AppContext.CancellationToken);
                if (affected <= 0 && hasRowGuard)
                {
                    throw new EntityNotFoundException();
                }
            }
            catch (DbException exception)
            {
                HandleException(ExceptionDispatchInfo.Capture(exception));
                throw;
            }
        }

        /// <summary>
        ///     Execute command and return scalar result.
        /// </summary>
        /// <param name="resultGuard">Throw if no result was returned.</param>
        public async Task<TResult> ScalarAsync<TResult>(bool resultGuard = true)
        {
            try
            {
                var result = await Command.ExecuteScalarAsync(AppContext.CancellationToken);
                if (result == null && resultGuard)
                {
                    throw new EntityNotFoundException();
                }

                return (TResult)result;
            }
            catch (DbException exception)
            {
                HandleException(ExceptionDispatchInfo.Capture(exception));
                throw;
            }
        }

        /// <summary>
        ///     Dispose unmanaged resources.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            // NOTE: Cannot dispose async with nullable (?.) check.
            if (Reader != null)
            {
                await Reader.DisposeAsync();
            }

            await Command.DisposeAsync();
            await Connection.DisposeAsync();
        }
    }
}
