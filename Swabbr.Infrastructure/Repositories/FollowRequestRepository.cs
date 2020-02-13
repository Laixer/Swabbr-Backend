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

        public Task<FollowRequest> GetByUserIdAsync(Guid receiverId, Guid requesterId)
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
    }

}

