using Dapper;
using Laixer.Infra.Npgsql;
using Laixer.Utility.Extensions;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Contains reusable repository functionality.
    /// </summary>
    internal static class SharedRepositoryFunctions
    {

        /// <summary>
        /// Checks if an entity exists in our database.
        /// TODO Sending table string might be dangerous?
        /// </summary>
        /// <param name="provider"><see cref="IDatabaseProvider"/></param>
        /// <param name="tableName">Table name in our database</param>
        /// <param name="id">Internal entity id</param>
        /// <returns><see cref="true"/> if the entity exists</returns>
        internal static async Task<bool> ExistsAsync(IDatabaseProvider provider, string tableName, Guid id)
        {
            if (provider == null) { throw new ArgumentNullException(nameof(provider)); }
            tableName.ThrowIfNullOrEmpty();
            id.ThrowIfNullOrEmpty();

            using (var connection = provider.GetConnectionScope())
            {
                var sql = $"SELECT 1 FROM {tableName} WHERE id = '{id}';";
                var result = await connection.QueryAsync<int>(sql).ConfigureAwait(false);
                if (result == null) { throw new InvalidOperationException("Result for exist checks should never be null"); }
                if (result.Count() > 1) { throw new InvalidOperationException("Single check result should never have more than one entity"); }
                return result.Count() == 1;
            }
        }

        /// <summary>
        /// Gets a single <typeparamref name="TEntity"/> from our database.
        /// </summary>
        /// <remarks>
        /// TODO Question is this safe? 
        /// </remarks>
        /// <typeparam name="TEntity"><see cref="EntityBase"/></typeparam>
        /// <param name="provider"><see cref="IDatabaseProvider"/></param>
        /// <param name="id">Internal <typeparamref name="TEntity"/> id</param>
        /// <param name="tableName">Name of our table</param>
        /// <returns></returns>
        internal static async Task<TEntity> GetAsync<TEntity>(IDatabaseProvider provider, Guid id, string tableName)
            where TEntity : EntityBase<Guid>
        {
            id.ThrowIfNullOrEmpty();
            tableName.ThrowIfNullOrEmpty();
            if (provider == null) { throw new ArgumentNullException(nameof(provider)); }

            using (var connection = provider.GetConnectionScope())
            {
                var sql = $"SELECT * FROM {tableName} WHERE id = @Id FOR UPDATE";
                var result = await connection.QueryAsync<TEntity>(sql, new { Id = id }).ConfigureAwait(false);

                if (result == null || !result.Any()) { throw new EntityNotFoundException(); }
                if (result.Count() > 1) { throw new InvalidOperationException("Found more than one entity for single get"); }
                return result.First();
            }
        }

        /// <summary>
        /// Deletes an <see cref="EntityBase{TPrimary}"/> from our database.
        /// </summary>
        /// <param name="provider"><see cref="IDatabaseProvider"/></param>
        /// <param name="id">Internal id</param>
        /// <param name="tableName">Name of our table</param>
        /// <returns><see cref="Task"/></returns>
        internal static async Task DeleteAsync(IDatabaseProvider provider, Guid id, string tableName)
        {
            id.ThrowIfNullOrEmpty();
            tableName.ThrowIfNullOrEmpty();
            if (provider == null) { throw new ArgumentNullException(nameof(provider)); }

            using (var connection = provider.GetConnectionScope())
            {
                var sql = $"DELETE FROM {tableName} WHERE id = @Id";
                var result = await connection.ExecuteAsync(sql, new { Id = id }).ConfigureAwait(false);

                if (result < 0) { throw new InvalidOperationException("Number of rows deleted can never be < 0"); }
                if (result > 1) { throw new InvalidOperationException("Found more than one entity for single get"); }
                if (result == 0) { throw new EntityNotFoundException("Could not find entity to delete"); }
            }
        }

    }

}
