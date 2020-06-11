using Dapper;
using Laixer.Infra.Npgsql;
using Laixer.Utility.Extensions;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using static Swabbr.Infrastructure.Database.DatabaseConstants;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="Vlog"/> entities.
    /// </summary>
    public sealed class VlogRepository : IVlogRepository
    {

        private readonly IDatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
        }

        /// <summary>
        /// Adds a single view to a <see cref="Vlog"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task AddView(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    UPDATE {TableVlog}
                    SET views = views + 1
                    WHERE id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = vlogId }).ConfigureAwait(false);
            if (rowsAffected == 0) { throw new EntityNotFoundException(nameof(Vlog)); }
            if (rowsAffected > 1) { throw new MultipleEntitiesFoundException(nameof(Vlog)); }
        }

        /// <summary>
        /// Creates a new <see cref="Vlog"/> in our database.
        /// </summary>
        /// <param name="entity"><see cref="Vlog"/></param>
        /// <returns><see cref="Vlog"/> with id assigned to it</returns>
        public async Task<Vlog> CreateAsync(Vlog entity)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
            entity.Id.ThrowIfNotNullOrEmpty();
            entity.UserId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            // TODO DRY
            // TODO Inserting a new vlog that is a reaction should never get a livestream id
            // This will create a possibility where the livestream_id will be {0000-00-00...}.
            var sql = $@"
                    INSERT INTO {TableVlog} (
                        is_private,
                        user_id,
                        livestream_id
                    ) VALUES (
                        @IsPrivate,
                        @UserId,
                        @LivestreamId
                    ) RETURNING id";
            var id = await connection.ExecuteScalarAsync<Guid>(sql, entity).ConfigureAwait(false);
            id.ThrowIfNullOrEmpty();
            entity.Id = id;
            return entity;
        }

        /// <summary>
        /// Checks if a given <see cref="Vlog"/> exists.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns>Bool result</returns>
        public async Task<bool> ExistsAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT * FROM {TableVlog} 
                    WHERE id = @Id 
                    AND vlog_state != '{VlogState.Deleted.GetEnumMemberAttribute()}'";
            var result = await connection.QueryAsync<Vlog>(sql, new { Id = vlogId }).ConfigureAwait(false);
            if (result == null || !result.Any()) { throw new EntityNotFoundException(); }
            if (result.Count() > 1) { throw new InvalidOperationException("Found more than one entity for single get"); }
            return result.Count() == 1;
        }

        /// <summary>
        /// Checks if a <see cref="Vlog"/> exists for a specified <see cref="Livestream"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="true"/> if exists</returns>
        public async Task<bool> ExistsForLivestreamAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT 1 FROM {TableVlog} 
                    WHERE livestream_id = @Id
                    AND vlog_state != '{VlogState.Deleted.GetEnumMemberAttribute()}'";
            var pars = new { Id = livestreamId };
            var result = await connection.QueryAsync<int>(sql, pars).ConfigureAwait(false);
            if (result == null || !result.Any()) { return false; }
            if (result.Count() > 1) { throw new MultipleEntitiesFoundException(nameof(Vlog)); }
            return true;
        }

        /// <summary>
        /// Gets a <see cref="Vlog"/> from our database.
        /// </summary>
        /// <param name="id">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Vlog"/>/returns>
        public async Task<Vlog> GetAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT * 
                    FROM {TableVlog} 
                    WHERE id = @Id 
                    AND vlog_state != '{VlogState.Deleted.GetEnumMemberAttribute()}'";
            var result = await connection.QueryAsync<Vlog>(sql, new { Id = id }).ConfigureAwait(false);
            if (result == null || !result.Any()) { throw new EntityNotFoundException(nameof(Vlog)); }
            if (result.Count() > 1) { throw new MultipleEntitiesFoundException(nameof(Vlog)); }
            return result.First();
        }

        public Task<IEnumerable<Vlog>> GetFeaturedVlogsAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a collection of <see cref="Vlog"/>s based on a users 
        /// following.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="maxCount">Max result count</param>
        /// <returns><see cref="Vlog"/> collection</returns>
        public async Task<IEnumerable<Vlog>> GetMostRecentVlogsForUserAsync(Guid userId, uint maxCount)
        {
            userId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            // TODO SQL injection for enum
            var sql = $@"
                    SELECT v.* FROM {TableVlog} AS v
                    JOIN {TableFollowRequest} AS f
                        ON v.user_id = f.receiver_id
                    WHERE f.requester_id = @UserId
                        AND f.follow_request_status = '{FollowRequestStatus.Accepted.GetEnumMemberAttribute()}'
                        AND v.vlog_state = '{VlogState.UpToDate.GetEnumMemberAttribute()}'
                    ORDER BY v.start_date DESC
                    LIMIT @MaxCount";
            var pars = new { UserId = userId, MaxCount = (int)maxCount };
            return await connection.QueryAsync<Vlog>(sql, pars).ConfigureAwait(false);
        }

        public Task<int> GetVlogCountForUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a <see cref="Vlog"/> based on a <see cref="Livestream"/>, since
        /// they are 1-1 linked.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Vlog"/> linked to the <see cref="Livestream"/></returns>
        public async Task<Vlog> GetVlogFromLivestreamAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT * 
                    FROM {TableVlog} 
                    WHERE livestream_id = @LivestreamId 
                    AND vlog_state != '{VlogState.Deleted.GetEnumMemberAttribute()}'";
            var pars = new { LivestreamId = livestreamId };
            var result = await connection.QueryAsync<Vlog>(sql, pars).ConfigureAwait(false);
            if (result == null || !result.Any()) { throw new EntityNotFoundException(nameof(Vlog)); }
            if (result.Count() > 1) { throw new MultipleEntitiesFoundException(nameof(Vlog)); }
            return result.First();
        }

        /// <summary>
        /// Gets a <see cref="Vlog"/> based on a <see cref="Reaction"/>.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Vlog"/></returns>
        public async Task<Vlog> GetVlogFromReactionAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT v.* 
                    FROM {TableReaction} AS r
                    JOIN {TableVlog} AS v
                        ON r.target_vlog_id = v.id
                    WHERE r.id = @ReactionId
                        AND v.vlog_state != '{VlogState.Deleted.GetEnumMemberAttribute()}'";
            var result = await connection.QueryAsync<Vlog>(sql, new { ReactionId = reactionId }).ConfigureAwait(false);
            if (result == null || !result.Any()) { throw new EntityNotFoundException(nameof(Vlog)); }
            if (result.Count() > 1) { throw new MultipleEntitiesFoundException(nameof(Vlog)); }
            return result.First();
        }

        /// <summary>
        /// Gets all <see cref="Vlog"/> entities owned by a specified
        /// <see cref="SwabbrUser"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Vlog"/> collection</returns>
        public async Task<IEnumerable<Vlog>> GetVlogsFromUserAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT * 
                    FROM {TableVlog} 
                    WHERE user_id = @UserId
                    AND vlog_state != '{VlogState.Deleted.GetEnumMemberAttribute()}'";
            return await connection.QueryAsync<Vlog>(sql, new { UserId = userId }).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes a <see cref="Vlog"/> from our database.
        /// </summary>
        /// <remarks>
        /// This throws an <see cref="Core.Exceptions.EntityNotFoundException"/> if we can't find the entity.
        /// </remarks>
        /// <param name="id">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task HardDeleteAsync(Guid id)
        {
            return SharedRepositoryFunctions.DeleteAsync(_databaseProvider, id, TableVlog);
        }

        /// <summary>
        /// Marks a <see cref="Vlog"/> as <see cref="VlogState.Deleted"/>.
        /// </summary>
        /// <param name="id">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task SoftDeleteAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $"UPDATE {TableVlog} SET vlog_state = '{VlogState.Deleted.GetEnumMemberAttribute()}' WHERE id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id }).ConfigureAwait(false);
            if (rowsAffected == 0) { throw new EntityNotFoundException(nameof(Vlog)); }
            if (rowsAffected > 1) { throw new MultipleEntitiesFoundException(nameof(Vlog)); }
        }

        /// <summary>
        /// Updates a <see cref="Vlog"/> in our database.
        /// </summary>
        /// <param name="entity"><see cref="Vlog"/></param>
        /// <returns>Updated and queried <see cref="Vlog"/></returns>
        public async Task<Vlog> UpdateAsync(Vlog entity)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
            entity.Id.ThrowIfNullOrEmpty();

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            using var connection = _databaseProvider.GetConnectionScope();
            await GetAsync(entity.Id).ConfigureAwait(false); // FOR UPATE

            var sql = $@"
                        UPDATE {TableVlog} SET
                            is_private = @IsPrivate
                        WHERE id = @Id
                        AND vlog_state != '{VlogState.Deleted.GetEnumMemberAttribute()}'";
            int rowsAffected = await connection.ExecuteAsync(sql, entity).ConfigureAwait(false);
            if (rowsAffected <= 0) { throw new EntityNotFoundException(); }
            if (rowsAffected > 1) { throw new InvalidOperationException("Found multiple results on single get"); }

            var result = await GetAsync(entity.Id).ConfigureAwait(false);
            scope.Complete();
            return result;
        }

    }

}
