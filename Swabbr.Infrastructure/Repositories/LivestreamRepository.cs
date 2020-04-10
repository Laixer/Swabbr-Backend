using Dapper;
using Laixer.Infra.Npgsql;
using Laixer.Utility.Extensions;
using Npgsql;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Utility;
using System;
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

        private readonly IDatabaseProvider _databaseProvider;

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
                // TODO For some weird reason dapper broke
                var sql = $@"
                    SELECT 
                        id AS Id, 
                        broadcast_location AS BroadcastLocation,
                        create_date AS CreateDate,
                        name AS Name,
                        update_date AS UpdateDate,
                        user_id AS UserId, 
                        external_id AS ExternalId,
                        livestream_status AS LivestreamStatus,
                        user_trigger_minute AS UserTriggerMinute
                    FROM {TableLivestream} 
                    WHERE id = @Id FOR UPDATE";
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
            entity.UserId.ThrowIfNotNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                // TODO Skipping vlog id --> might be bug-sensitive
                // TODO This inserts null as the user minute and user id explicitly. Might be bug-sensitive
                var sql = $@"
                    INSERT INTO {TableLivestream} (
                        broadcast_location,
                        create_date,
                        external_id,
                        name,
                        user_id,
                        user_trigger_minute
                    ) VALUES (
                        @BroadcastLocation,
                        @CreateDate,
                        @ExternalId,
                        @Name,
                        null,
                        null
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
            if (status == LivestreamStatus.PendingUser) { throw new InvalidOperationException($"Use {nameof(MarkPendingUserAsync)} instead for {status.GetEnumMemberAttribute()}"); }

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                // TODO SQL injection
                var sql = $"UPDATE {TableLivestream} SET livestream_status = '{status.GetEnumMemberAttribute()}' WHERE id = @Id";
                var pars = new { Id = id};
                var rowsAffected = await connection.ExecuteAsync(sql, pars).ConfigureAwait(false);
                if (rowsAffected == 0) { throw new EntityNotFoundException(); }
                if (rowsAffected > 1) { throw new MultipleEntitiesFoundException(); }
            }
        }

        /// <summary>
        /// Sets the user for a <see cref="Livestream"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task MarkPendingUserAsync(Guid livestreamId, Guid userId, DateTimeOffset triggerMinute)
        {
            livestreamId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();
            if (triggerMinute == null) { throw new ArgumentNullException(nameof(triggerMinute)); }

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    UPDATE {TableLivestream} SET
                        user_id = @UserId,
                        livestream_status = '{LivestreamStatus.PendingUser.GetEnumMemberAttribute()}',
                        user_trigger_minute = @UserTriggerMinute
                    WHERE id = @Id";
                var pars = new { Id = livestreamId, UserId = userId, UserTriggerMinute = triggerMinute.GetMinutes()};
                await connection.ExecuteAsync(sql, pars).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets a livestream based on a trigger minute.
        /// </summary>
        /// <remarks>
        /// TODO For some reason the dapper mapping does not work at all for this livestream, hence the call to <see cref="GetAsync(Guid)"/>.
        /// </remarks>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="triggerMinute"><see cref="DateTimeOffset"/></param>
        /// <returns><see cref="Livestream"/></returns>
        public async Task<Livestream> GetLivestreamFromTriggerMinute(Guid userId, DateTimeOffset triggerMinute)
        {
            userId.ThrowIfNullOrEmpty();
            if (triggerMinute == null) { throw new ArgumentNullException(nameof(triggerMinute)); }

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    SELECT id FROM {TableLivestream}
                    WHERE user_id = @UserId
                    AND user_trigger_minute = @UserTriggerMinute";
                // TODO FOR UPDATE
                var pars = new { UserId = userId, UserTriggerMinute = triggerMinute.GetMinutes() };
                var result = await connection.QueryAsync<Guid>(sql, pars).ConfigureAwait(false);
                if (result == null || !result.Any()) { throw new EntityNotFoundException(); }
                if (result.Count() > 1) { throw new MultipleEntitiesFoundException(); }
                var actual = await GetAsync(result.First()).ConfigureAwait(false);
                return actual;
            }
        }

    }

}
