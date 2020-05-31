using Dapper;
using Laixer.Infra.Npgsql;
using Laixer.Utility.Extensions;
using Microsoft.Extensions.Logging;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Utility;
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

        private readonly IDatabaseProvider _databaseProvider;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public LivestreamRepository(IDatabaseProvider databaseProvider,
            ILoggerFactory loggerFactory)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(LivestreamRepository)) : throw new ArgumentNullException(nameof(loggerFactory));
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

            using var connection = _databaseProvider.GetConnectionScope();
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

        /// <summary>
        /// Deletes a <see cref="Livestream"/> from our database.
        /// </summary>
        /// <param name="id">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task DeleteAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $"DELETE FROM {TableLivestream} WHERE id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id }).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks if a livestream exists for a user-triggerminute combination.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="triggerMinute"><see cref="DateTimeOffset"/></param>
        /// <returns><see cref="bool"/> result</returns>
        public async Task<bool> ExistsLivestreamForTriggerMinute(Guid userId, DateTimeOffset triggerMinute)
        {
            userId.ThrowIfNullOrEmpty();
            if (triggerMinute == null) { throw new ArgumentNullException(nameof(triggerMinute)); }

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT 1 FROM {TableLivestream}
                    WHERE user_id = @UserId
                    AND user_trigger_minute = @UserTriggerMinute
                    FOR UPDATE";
            var pars = new { UserId = userId, UserTriggerMinute = triggerMinute.GetMinutes() };

            var result = await connection.QueryAsync<int>(sql, pars).ConfigureAwait(false);
            if (result.Count() > 1) { throw new MultipleEntitiesFoundException(); }

            return result.Count() == 1;
        }

        /// <summary>
        /// Gets a <see cref="Livestream"/> from our database.
        /// </summary>
        /// <param name="id">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Livestream"/></returns>
        public async Task<Livestream> GetAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
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
                        livestream_state AS LivestreamState,
                        user_trigger_minute AS UserTriggerMinute
                    FROM {TableLivestream} 
                    WHERE id = @Id FOR UPDATE";
            var result = await connection.QueryAsync<Livestream>(sql, new { Id = id }).ConfigureAwait(false);

            if (result == null || !result.Any()) { throw new EntityNotFoundException(); }
            if (result.Count() > 1) { throw new InvalidOperationException("Found more than one entity for single get"); }
            return result.First();
        }

        /// <summary>
        /// Gets all <see cref="Livestream"/> entities from the database that
        /// are currently available for claiming.
        /// </summary>
        /// <remarks>
        /// Uses the FOR UPDATE sql clause.
        /// </remarks>
        /// <returns><see cref="Livestream"/> collection</returns>
        public async Task<IEnumerable<Livestream>> GetAvailableLivestreamsAsync()
        {
            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT
                        id AS Id, 
                        broadcast_location AS BroadcastLocation,
                        create_date AS CreateDate,
                        name AS Name,
                        update_date AS UpdateDate,
                        user_id AS UserId, 
                        external_id AS ExternalId,
                        livestream_state AS LivestreamState,
                        user_trigger_minute AS UserTriggerMinute
                    FROM {TableLivestream}
                    WHERE livestream_state = '{LivestreamState.Created.GetEnumMemberAttribute()}'
                    FOR UPDATE";
            return await connection.QueryAsync<Livestream>(sql).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a <see cref="Livestream"/> by its external id.
        /// </summary>
        /// <param name="externalId">External <see cref="Livestream"/> id</param>
        /// <returns><see cref="Livestream"/></returns>
        public async Task<Livestream> GetByExternalIdAsync(string externalId)
        {
            externalId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $"SELECT * FROM {TableLivestream} WHERE external_id = @ExternalId";
            var result = await connection.QueryAsync<Livestream>(sql, new { ExternalId = externalId }).ConfigureAwait(false);
            if (result == null || !result.Any()) { throw new EntityNotFoundException(nameof(Livestream)); }
            if (result.Count() > 1) { throw new MultipleEntitiesFoundException(nameof(Livestream)); }
            return result.First();
        }

        /// <summary>
        /// Gets the <see cref="Livestream.ExternalId"/> property from our database.
        /// </summary>
        /// <param name="id">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Livestream.ExternalId"/> string value</returns>
        public async Task<string> GetExternalIdAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT external_id FROM {TableLivestream}
                    WHERE id = @Id
                    FOR UPDATE";
            var pars = new { Id = id };
            var result = await connection.QueryAsync<string>(sql, pars).ConfigureAwait(false);
            if (result == null || !result.Any()) { throw new EntityNotFoundException(nameof(Livestream)); }
            if (result.Count() > 1) { throw new MultipleEntitiesFoundException(nameof(Livestream)); }
            result.First().ThrowIfNullOrEmpty();
            return result.First();
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

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT id FROM {TableLivestream}
                    WHERE user_id = @UserId
                    AND user_trigger_minute = @UserTriggerMinute
                    FOR UPDATE";
            var pars = new { UserId = userId, UserTriggerMinute = triggerMinute.GetMinutes() };

            var result = await connection.QueryAsync<Guid>(sql, pars).ConfigureAwait(false);
            if (result == null || !result.Any()) { throw new EntityNotFoundException(nameof(Livestream)); }
            if (result.Count() > 1) { throw new MultipleEntitiesFoundException(nameof(Livestream)); }
            var actual = await GetAsync(result.First()).ConfigureAwait(false);
            return actual;
        }

        /// <summary>
        /// Checks if a <see cref="SwabbrUser"/> is in a livestream cycle.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="bool"/> result</returns>
        public async Task<bool> IsUserInLivestreamCycleAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT 1 FROM {TableLivestream}
                    WHERE user_id = @UserId
                    FOR UPDATE";
            var count = await connection.QueryAsync<int>(sql, new { UserId = userId }).ConfigureAwait(false);
            if (count.Count() > 1) { throw new InvalidOperationException("Found multiple livestreams with the same user_id"); }
            return count.Any();
        }

        /// <summary>
        /// Marks a <see cref="Livestream"/> as <see cref="LivestreamState.Closed"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task MarkClosedAsync(Guid livestreamId)
        {
            return MarkAsStateAsync(livestreamId, LivestreamState.Closed);
        }

        /// <summary>
        /// Marks a <see cref="Livestream"/> as created.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <param name="externalId">External <see cref="Livestream"/> id</param>
        /// <param name="broadcastLocation">External broadcasting location name</param>
        /// <returns><see cref="Task"/></returns>
        public async Task MarkCreatedAsync(Guid livestreamId, string externalId, string broadcastLocation)
        {
            livestreamId.ThrowIfNullOrEmpty();
            externalId.ThrowIfNullOrEmpty();
            broadcastLocation.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    UPDATE {TableLivestream} SET
                        broadcast_location = @BroadcastLocation,
                        external_id = @ExternalId,
                        livestream_state = '{LivestreamState.Created.GetEnumMemberAttribute()}'
                    WHERE id = @Id";
            var pars = new { Id = livestreamId, BroadcastLocation = broadcastLocation, ExternalId = externalId };
            var rowsAffected = await connection.ExecuteAsync(sql, pars).ConfigureAwait(false);
            if (rowsAffected <= 0) { throw new EntityNotFoundException(nameof(Livestream)); }
            if (rowsAffected > 1) { throw new MultipleEntitiesFoundException(nameof(Livestream)); }
        }

        /// <summary>
        /// Marks a <see cref="Livestream"/> as <see cref="LivestreamState.Live"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task MarkLiveAsync(Guid livestreamId)
        {
            return MarkAsStateAsync(livestreamId, LivestreamState.Live);
        }

        /// <summary>
        /// Marks a <see cref="Livestream"/> as <see cref="LivestreamState.PendingClosure"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task MarkPendingClosureAsync(Guid livestreamId)
        {
            return MarkAsStateAsync(livestreamId, LivestreamState.PendingClosure);
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

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    UPDATE {TableLivestream} SET
                        user_id = @UserId,
                        livestream_state = '{LivestreamState.PendingUser.GetEnumMemberAttribute()}',
                        user_trigger_minute = @UserTriggerMinute
                    WHERE id = @Id";
            var pars = new { Id = livestreamId, UserId = userId, UserTriggerMinute = triggerMinute.GetMinutes() };
            var rowsAffected = await connection.ExecuteAsync(sql, pars).ConfigureAwait(false);
            if (rowsAffected <= 0) { throw new EntityNotFoundException(nameof(Livestream)); }
            if (rowsAffected > 1) { throw new MultipleEntitiesFoundException(nameof(Livestream)); }
        }

        /// <summary>
        /// Marks a <see cref="Livestream"/> as <see cref="LivestreamState.PendingUserConnect"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task MarkPendingUserConnectAsync(Guid livestreamId)
        {
            return MarkAsStateAsync(livestreamId, LivestreamState.PendingUserConnect);
        }

        /// <summary>
        /// Marks a <see cref="Livestream"/> as <see cref="LivestreamState.UserNeverConnectedTimeout"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task MarkUserNeverConnectedTimeoutAsync(Guid livestreamId)
        {
            return MarkAsStateAsync(livestreamId, LivestreamState.UserNeverConnectedTimeout);
        }

        /// <summary>
        /// Marks a <see cref="Livestream"/> as <see cref="LivestreamState.UserNoResponseTimeout"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task MarkUserNoResponseTimeoutAsync(Guid livestreamId)
        {
            return MarkAsStateAsync(livestreamId, LivestreamState.UserNoResponseTimeout);
        }

        /// <summary>
        /// Sets a given <see cref="Livestream"/> as <paramref name="state"/>.
        /// </summary>
        /// <remarks>
        /// This throws if the database doesn't allow the state change.
        /// </remarks>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <param name="state">New <see cref="LivestreamState"/></param>
        /// <returns><see cref="Task"/></returns>
        private async Task MarkAsStateAsync(Guid livestreamId, LivestreamState state)
        {
            livestreamId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    UPDATE {TableLivestream} 
                    SET livestream_state = '{state.GetEnumMemberAttribute()}'
                    WHERE id = @LivestreamId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { LivestreamId = livestreamId }).ConfigureAwait(false);
            if (rowsAffected <= 0) { throw new EntityNotFoundException(nameof(Livestream)); }
            if (rowsAffected > 1) { throw new MultipleEntitiesFoundException(nameof(Livestream)); }
        }

    }

}
