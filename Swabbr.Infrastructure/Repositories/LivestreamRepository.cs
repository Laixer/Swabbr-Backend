using Dapper;
using Laixer.Infra.Npgsql;
using Laixer.Utility.Extensions;
using Npgsql;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Swabbr.Infrastructure.Database.DatabaseConstants;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="Livestream"/> entities.
    /// </summary>
    public sealed class LivestreamRepository : ILivestreamRepository
    {

        private IDatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public LivestreamRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
        }

        /// <summary>
        /// Gets a <see cref="Livestream"/> from our database.
        /// </summary>
        /// <param name="id">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Livestream"/></returns>
        public async Task<Livestream> GetAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                // TODO For some weird reason dapper doesn't map ExternalId...
                var sql = $"SELECT *, external_id AS ExternalId FROM {TableLivestream} WHERE id = @Id FOR UPDATE";
                var result = await connection.QueryAsync<Livestream>(sql, new { Id = id }).ConfigureAwait(false);

                if (result == null || !result.Any()) { throw new EntityNotFoundException(); }
                if (result.Count() > 1) { throw new InvalidOperationException("Found more than one entity for single get"); }
                return result.First();
            }
        }

        /// <summary>
        /// Creates a <see cref="Livestream"/> entity in our database.
        /// </summary>
        /// <param name="entity"><see cref="Livestream"/></param>
        /// <returns><see cref="Livestream"/> with id assigned</returns>
        public async Task<Livestream> CreateAsync(Livestream entity)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
            entity.Id.ThrowIfNotNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                // TODO Skipping user id and vlog id --> might be bug-sensitive
                var sql = $@"
                    INSERT INTO {TableLivestream} (
                        broadcast_location,
                        create_date,
                        external_id,
                        name,
                        user_id
                    ) VALUES (
                        @BroadcastLocation,
                        @CreateDate,
                        @ExternalId,
                        @Name,
                        @UserId
                    ) RETURNING id";
                var id = await connection.ExecuteScalarAsync<Guid>(sql, entity).ConfigureAwait(false);
                id.ThrowIfNullOrEmpty();
                entity.Id = id;
                return entity;
            }
        }

        /// <summary>
        /// Deletes a <see cref="Livestream"/> from our database.
        /// </summary>
        /// <param name="id">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task DeleteAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"DELETE FROM {TableLivestream} WHERE id = @Id";
                await connection.ExecuteAsync(sql, new { Id = id }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the <see cref="Livestream.ExternalId"/> property from our database.
        /// </summary>
        /// <param name="id">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Livestream.ExternalId"/> string value</returns>
        public async Task<string> GetExternalIdAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    SELECT external_id FROM {TableLivestream}
                    WHERE id = @Id
                    FOR UPDATE";
                var pars = new { Id = id };
                var result = await connection.QueryAsync<string>(sql, pars).ConfigureAwait(false);
                if (result == null || !result.Any()) { throw new EntityNotFoundException(); }
                if (result.Count() > 1) { throw new InvalidOperationException("Found multiple result for single get"); }
                result.First().ThrowIfNullOrEmpty();
                return result.First();
            }
        }

        public Task<Livestream> UpdateAsync(Livestream entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the <see cref="Livestream.LivestreamStatus"/> property in our database.
        /// </summary>
        /// <param name="id">Internal <see cref="Livestream"/> id</param>
        /// <param name="status">New <see cref="LivestreamStatus"/></param>
        /// <returns><see cref="Task"/></returns>
        public async Task UpdateLivestreamStatusAsync(Guid id, LivestreamStatus status)
        {
            id.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                // TODO This is unsafe because we cast to NpgsqlConnection!
                connection.Open();
                var sql = $"UPDATE {TableLivestream} SET livestream_status = @Status WHERE id = @Id";
                using (var command = new NpgsqlCommand(sql, connection as NpgsqlConnection))
                {
                    command.Parameters.AddWithValue("Id", id);
                    command.Parameters.AddWithValue("Status", status);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }

            }
        }
    }

}
