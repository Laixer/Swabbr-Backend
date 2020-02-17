using Laixer.Infra.Npgsql;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laixer.Utility.Extensions;
using Dapper;
using System.Linq;
using Swabbr.Core.Exceptions;

using static Swabbr.Infrastructure.Database.DatabaseConstants;
using Swabbr.Core.Enums;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="FollowRequest"/> entities.
    /// </summary>
    public sealed class FollowRequestRepository : IFollowRequestRepository
    {

        private IDatabaseProvider _databaseProvider;

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
        /// <param name="id">Internal id</param>
        /// <returns><see cref="FollowRequest"/></returns>
        public async Task<FollowRequest> GetAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();
            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"SELECT * FROM {TableFollowRequest} WHERE id = '{id}';";
                var result = await connection.QueryAsync<FollowRequest>(sql);
                if (result == null || !result.Any()) { throw new EntityNotFoundException($"Could not find FollowRequest with id = {id}"); }
                else return result.First();
            }
        }

        /// <summary>
        /// Checks if a given follow request exists between a receiver and a
        /// requester.
        /// </summary>
        /// <param name="receiverId">Receiving <see cref="SwabbrUser"/> internal id</param>
        /// <param name="requesterId">Requesting <see cref="SwabbrUser"/> internal id</param>
        /// <returns><see cref="true"/> if a <see cref="FollowRequest"/> exists</returns>
        public async Task<bool> ExistsAsync(Guid receiverId, Guid requesterId)
        {
            receiverId.ThrowIfNullOrEmpty();
            requesterId.ThrowIfNullOrEmpty();
            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"SELECT 1 FROM {TableFollowRequest}" +
                    $" WHERE receiver_id = '{receiverId}'" +
                    $" AND WHERE requester_id = {requesterId};";
                var result = await connection.QueryAsync<int>(sql);
                return result != null && result.Any();
            }
        }

        public Task<FollowRequest> GetByUserIdsAsync(Guid receiverId, Guid requesterId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetFollowerCountAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetFollowingCountAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FollowRequest>> GetIncomingForUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FollowRequest>> GetOutgoingForUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<FollowRequest> CreateAsync(FollowRequest entity)
        {
            throw new NotImplementedException();
        }

        public Task<FollowRequest> UpdateAsync(FollowRequest entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(FollowRequest entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the status for a <see cref="FollowRequest"/> to the 
        /// specified <see cref="FollowRequestStatus"/>. This will throw an
        /// <see cref="EntityNotFoundException"/> if we can't find the object.
        /// </summary>
        /// <param name="id">Internal <see cref="FollowRequest"/> id</param>
        /// <param name="status"><see cref="FollowRequestStatus"/></param>
        /// <returns><see cref="Task"/></returns>
        public async Task<FollowRequest> UpdateStatusAsync(Guid id, FollowRequestStatus status)
        {
            id.ThrowIfNullOrEmpty();
            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"UPDATE {TableFollowRequest}" +
                    $" SET follow_request_status = '{status.ToString()}'" + // TODO THOMAS To string might be dangerous? Mapping?!
                    $" WHERE id = {id};";
                var rowsAffected = await connection.ExecuteAsync(sql);
                if (rowsAffected == 0) { throw new EntityNotFoundException(nameof(FollowRequest)); }
                else if (rowsAffected > 1) { throw new InvalidOperationException($"Affected {rowsAffected} while updating a single follow request, this should never happen"); }
                else return await GetAsync(id);
            }
        }
    }

}

