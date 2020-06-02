using Dapper;
using Laixer.Infra.Npgsql;
using Laixer.Utility.Extensions;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using Swabbr.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Swabbr.Infrastructure.Database.DatabaseConstants;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="FollowRequest"/> entities.
    /// </summary>
    public sealed class FollowRequestRepository : IFollowRequestRepository
    {

        private readonly IDatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public FollowRequestRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
        }

        /// <summary>
        /// Gets a single <see cref="FollowRequest"/> based on its internal id.
        /// </summary>
        /// <remarks>
        /// Throws an <see cref="EntityNotFoundException"/> if the entity doesn't exist.
        /// </remarks>
        /// <param name="followRequestId">Internal id</param>
        /// <returns><see cref="FollowRequest"/></returns>
        public async Task<FollowRequest> GetAsync(FollowRequestId followRequestId)
        {
            followRequestId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT * FROM {TableFollowRequest}  
                    WHERE receiver_id = @ReceiverId
                    AND requester_id = @RequesterId
                    FOR UPDATE";
            var result = await connection.QueryAsync<FollowRequest>(sql, followRequestId).ConfigureAwait(false);
            if (result == null || !result.Any()) { throw new EntityNotFoundException(); }
            if (result.Count() > 1) { throw new InvalidOperationException("Found more than one entity for single get"); }
            return result.First();
        }

        /// <summary>
        /// Checks if a given follow request exists between a receiver and a
        /// requester.
        /// </summary>
        /// <param name="followRequestId">Internal <see cref="FollowRequest"/> id</param>
        /// <returns><see cref="true"/> if a <see cref="FollowRequest"/> exists</returns>
        public async Task<bool> ExistsAsync(FollowRequestId followRequestId)
        {
            followRequestId.ThrowIfNullOrEmpty();
            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $"SELECT 1 FROM {TableFollowRequest}" +
$" WHERE receiver_id = @ReceiverId" +
$" AND requester_id = @RequesterId";
            var result = await connection.QueryAsync<int>(sql, followRequestId).ConfigureAwait(false);
            return result != null && result.Any();
        }

        public Task<int> GetFollowerCountAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetFollowingCountAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lists incoming <see cref="FollowRequest"/>s.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="FollowRequest"/> collection</returns>
        public async Task<IEnumerable<FollowRequest>> GetIncomingForUserAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT * FROM {TableFollowRequest}
                    WHERE receiver_id = @UserId
                    AND follow_request_status = '{FollowRequestStatus.Pending.GetEnumMemberAttribute()}'
                    FOR UPDATE";
            return await connection.QueryAsync<FollowRequest>(sql, new { UserId = userId }).ConfigureAwait(false);
        }

        /// <summary>
        /// Lists outgoing <see cref="FollowRequest"/>s.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="FollowRequest"/> collection</returns>
        public async Task<IEnumerable<FollowRequest>> GetOutgoingForUserAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT * FROM {TableFollowRequest}
                    WHERE requester_id = @UserId
                    AND follow_request_status = '{FollowRequestStatus.Pending.GetEnumMemberAttribute()}'
                    FOR UPDATE";
            return await connection.QueryAsync<FollowRequest>(sql, new { UserId = userId }).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a single <see cref="FollowRequest"/> in our database.
        /// </summary>
        /// <remarks>
        /// This function does no boundary checks in our database.
        /// TODO Is this smart? Assuming that another entity will always do this?
        /// </remarks>
        /// <param name="entity"><see cref="FollowRequest"/></param>
        /// <returns>The created <see cref="FollowRequest"/> with id</returns>
        public async Task<FollowRequest> CreateAsync(FollowRequest entity)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
            entity.Id.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"INSERT INTO {TableFollowRequest} (
                        requester_id,
                        receiver_id
                    ) VALUES (
                        @RequesterId,
                        @ReceiverId
                    )";
            await connection.ExecuteAsync(sql, entity.Id).ConfigureAwait(false);

            return await GetAsync(entity.Id).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates a <see cref="FollowRequest"/> in our database.
        /// </summary>
        /// <remarks>
        /// At the moment this doesn't retrieve the followrequest.
        /// TODO Do we ever need this?
        /// </remarks>
        /// <param name="entity"><see cref="FollowRequest"/></param>
        /// <returns><see cref="FollowRequest"/></returns>
        public Task<FollowRequest> UpdateAsync(FollowRequest entity)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
            entity.Id.ThrowIfNullOrEmpty();

            return UpdateStatusAsync(entity.Id, entity.FollowRequestStatus);
        }

        /// <summary>
        /// Deletes a <see cref="FollowRequest"/> from our database.
        /// </summary>
        /// <param name="followRequestId">Internal <see cref="FollowRequest"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task DeleteAsync(FollowRequestId followRequestId)
        {
            followRequestId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    DELETE FROM {TableFollowRequest} 
                    WHERE requester_id = @RequesterId
                    AND receiver_id = @ReceiverId";
            var rowsAffected = await connection.ExecuteAsync(sql, followRequestId).ConfigureAwait(false);
            if (rowsAffected <= 0) { throw new EntityNotFoundException(nameof(FollowRequest)); }
            if (rowsAffected > 1) { throw new MultipleEntitiesFoundException(nameof(FollowRequest)); }
        }

        /// <summary>
        /// Updates the status for a <see cref="FollowRequest"/> to the 
        /// specified <see cref="FollowRequestStatus"/>. This will throw an
        /// <see cref="EntityNotFoundException"/> if we can't find the object.
        /// </summary>
        /// <param name="followRequestId">Internal <see cref="FollowRequest"/> id</param>
        /// <param name="status"><see cref="FollowRequestStatus"/></param>
        /// <returns><see cref="Task"/></returns>
        public async Task<FollowRequest> UpdateStatusAsync(FollowRequestId followRequestId, FollowRequestStatus status)
        {
            followRequestId.ThrowIfNullOrEmpty();
            using var connection = _databaseProvider.GetConnectionScope();
            // TODO Anti SQL inject
            var sql = $@"
                    UPDATE {TableFollowRequest}
                    SET follow_request_status = '{status.GetEnumMemberAttribute()}'
                    WHERE requester_id = @RequesterId
                    AND receiver_id = @ReceiverId";
            var rowsAffected = await connection.ExecuteAsync(sql, followRequestId).ConfigureAwait(false);
            if (rowsAffected == 0) { throw new EntityNotFoundException(nameof(FollowRequest)); }
            else if (rowsAffected > 1) { throw new InvalidOperationException($"Affected {rowsAffected} while updating a single follow request, this should never happen"); }
            else
            {
                return await GetAsync(followRequestId).ConfigureAwait(false);
            }
        }

    }

}
