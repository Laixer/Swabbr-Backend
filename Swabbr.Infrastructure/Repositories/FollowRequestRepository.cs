using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using Swabbr.Infrastructure.Abstractions;
using Swabbr.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{
    /// <summary>
    ///     Repository for follow requests.
    /// </summary>
    internal class FollowRequestRepository : RepositoryBase, IFollowRequestRepository
    {
        public Task<FollowRequestId> CreateAsync(FollowRequest entity) => throw new NotImplementedException();
        
        public Task DeleteAsync(FollowRequestId id) => throw new NotImplementedException();
        public Task<bool> ExistsAsync(FollowRequestId id) => throw new NotImplementedException();
        public IAsyncEnumerable<FollowRequest> GetAllAsync(Navigation navigation) => throw new NotImplementedException();
        
        /// <summary>
        ///     Get a follow request from our data store.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FollowRequest> GetAsync(FollowRequestId id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var sql = @"
                    SELECT  requester_id,
                            receiver_id,
                            date_created,
                            date_updated,
                            follow_request_status
                    FROM    application.follow_request
                    WHERE   requester_id = @requester_id
                    AND     receiver_id = @receiver_id
                    LIMIt 1";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("requester_id", id.RequesterId);
            context.AddParameterWithValue("receiver_id", id.ReceiverId);

            await using var reader = await context.ReaderAsync();

            return MapFromReader(reader);
        }
        
        public Task<uint> GetFollowerCountAsync(Guid userId) => throw new NotImplementedException();
        public Task<uint> GetFollowingCountAsync(Guid userId) => throw new NotImplementedException();
        public IAsyncEnumerable<FollowRequest> GetIncomingForUserAsync(Guid userId, Navigation navigation) => throw new NotImplementedException();
        public IAsyncEnumerable<FollowRequest> GetOutgoingForUserAsync(Guid userId, Navigation navigation) => throw new NotImplementedException();
        public Task UpdateAsync(FollowRequest entity) => throw new NotImplementedException();
        public Task UpdateStatusAsync(FollowRequestId id, FollowRequestStatus status) => throw new NotImplementedException();

        /// <summary>
        ///     Maps a reader to a follow request.
        /// </summary>
        /// <param name="reader">The open reader.</param>
        /// <returns>The mapped follow request.</returns>
        private static FollowRequest MapFromReader(DbDataReader reader)
            => new FollowRequest
            {
                Id = new FollowRequestId
                {
                    RequesterId = reader.GetGuid(0),
                    ReceiverId = reader.GetGuid(1)
                },
                DateCreated = reader.GetDateTime(2),
                DateUpdated = reader.GetSafeDateTime(3),
                FollowRequestStatus = reader.GetFieldValue<FollowRequestStatus>(4)
            };
    }
}
