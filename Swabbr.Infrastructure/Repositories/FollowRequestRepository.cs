using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using Swabbr.Infrastructure.Abstractions;
using Swabbr.Infrastructure.Database;
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
        /// <summary>
        ///     Create a new follow request.
        /// </summary>
        /// <remarks>
        ///     This doesn't actually return the created entity
        ///     id since we already now said id.
        /// </remarks>
        /// <param name="entity">The follow request.</param>
        /// <returns>The created follow requests id.</returns>
        public async Task<FollowRequestId> CreateAsync(FollowRequest entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var sql = @"
                    INSERT INTO application.follow_request(
                        receiver_id,
                        requester_id
                    VALUES (
                        @receiver_id,
                        @requester_id)";

            await using var context = await CreateNewDatabaseContext(sql);

            MapToWriter(context, entity);

            await using var reader = await context.ReaderAsync();

            return entity.Id;
        }

        // TODO Soft delete this?
        /// <summary>
        ///     Deletes a follow request from our data store.
        /// </summary>
        /// <param name="id">Follow request id.</param>
        public async Task DeleteAsync(FollowRequestId id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var sql = @"
                    DELETE  
                    FROM    application.follow_request AS fr
                    WHERE   fr.receiver_id = @receiver_id
                    AND     fr.requester_id = @requester_id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("receiver_id", id.ReceiverId);
            context.AddParameterWithValue("requester_id", id.RequesterId);

            await context.NonQueryAsync();
        }

        /// <summary>
        ///     Checks if a follow request exists in our database.
        /// </summary>
        /// <param name="id">The id to check for.</param>
        public async Task<bool> ExistsAsync(FollowRequestId id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var sql = @"
                    SELECT  EXISTS (
                    SELECT  1
                    FROM    application.follow_request AS fr
                    WHERE   fr.receiver_id = @receiver_id
                    AND     fr.requester_id = @requester_id)";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("receiver_id", id.ReceiverId);
            context.AddParameterWithValue("requester_id", id.RequesterId);

            return await context.ScalarAsync<bool>();
        }

        // TODO Remove?
        public IAsyncEnumerable<FollowRequest> GetAllAsync(Navigation navigation) => throw new NotImplementedException();
        
        /// <summary>
        ///     Get a follow request from our data store.
        /// </summary>
        /// <param name="id">Follow request id.</param>
        /// <returns>The follow request.</returns>
        public async Task<FollowRequest> GetAsync(FollowRequestId id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var sql = @"
                    SELECT  fr.date_created,
                            fr.date_updated,
                            fr.follow_request_status,
                            fr.receiver_id,
                            fr.requester_id
                    FROM    application.follow_request AS fr
                    WHERE   fr.receiver_id = @receiver_id
                    AND     fr.requester_id = @requester_id
                    LIMIT   1";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("receiver_id", id.ReceiverId);
            context.AddParameterWithValue("requester_id", id.RequesterId);

            await using var reader = await context.ReaderAsync();

            return MapFromReader(reader);
        }

        /// <summary>
        ///     Gets the total amoun tof users that are 
        ///     following <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The follower count.</returns>
        public async Task<uint> GetFollowerCountAsync(Guid userId)
        {
            var sql = @"
                    SELECT  COUNT(*)
                    FROM    application.follow_request AS fr
                    WHERE   fr.receiver_id = @receiver_id
                    AND     fr.follow_request_status = 'accepted'";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("receiver_id", userId);

            return (uint) await context.ScalarAsync<int>();
        }

        /// <summary>
        ///     Gets the total amount of users that the 
        ///     <paramref name="userId"/> is following.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>Total following count.</returns>
        public async Task<uint> GetFollowingCountAsync(Guid userId)
        {
            var sql = @"
                    SELECT  COUNT(*)
                    FROM    application.follow_request AS fr
                    WHERE   fr.requester_id = @requester_id
                    AND     fr.follow_request_status = 'accepted'";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("requester_id", userId);

            return (uint)await context.ScalarAsync<int>();
        }

        /// <summary>
        ///     Gets incoming follow requests for a user.
        /// </summary>
        /// <param name="userId">The user that will be followed.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Incoming follow requests.</returns>
        public async IAsyncEnumerable<FollowRequest> GetIncomingForUserAsync(Guid userId, Navigation navigation)
        {
            var sql =@"
                    SELECT  fr.date_created,
                            fr.date_updated,
                            fr.follow_request_status,
                            fr.receiver_id,
                            fr.requester_id
                    FROM    application.follow_request AS fr
                    WHERE   fr.receiver_id = @receiver_id
                    AND     fr.follow_request_status = 'pending'" ;

            ConstructNavigation(ref sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("receiver_id", userId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets outgoing follow requests for a user.
        /// </summary>
        /// <param name="userId">The user that will follow.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Outgoing follow requests.</returns>
        public async IAsyncEnumerable<FollowRequest> GetOutgoingForUserAsync(Guid userId, Navigation navigation)
        {
            var sql = @"
                    SELECT  fr.date_created,
                            fr.date_updated,
                            fr.follow_request_status,
                            fr.receiver_id,
                            fr.requester_id
                    FROM    application.follow_request AS fr
                    WHERE   fr.requester_id = @requester_id
                    AND     fr.follow_request_status = 'pending'";

            ConstructNavigation(ref sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("requester_id", userId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        // TODO Remove?
        public Task UpdateAsync(FollowRequest entity) => throw new NotImplementedException();

        /// <summary>
        ///     Updates the status of a follow request.
        /// </summary>
        /// <param name="id">The follow request id.</param>
        /// <param name="status">The new status.</param>
        public async Task UpdateStatusAsync(FollowRequestId id, FollowRequestStatus status)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var sql = @"
                    UPDATE  application.follow_request AS fr
                    SET     follow_request_status = @follow_request_status
                    WHERE   fr.receiver_id = @receiver_id
                    AND     fr.requester_id = @requester_id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("follow_request_status", status);
            context.AddParameterWithValue("receiver_id", id.ReceiverId);
            context.AddParameterWithValue("requester_id", id.RequesterId);

            await context.NonQueryAsync();
        }

        /// <summary>
        ///     Maps a reader to a follow request.
        /// </summary>
        /// <param name="reader">The open reader.</param>
        /// <returns>The mapped follow request.</returns>
        private static FollowRequest MapFromReader(DbDataReader reader)
            => new FollowRequest
            {
                DateCreated = reader.GetDateTime(0),
                DateUpdated = reader.GetSafeDateTime(1),
                FollowRequestStatus = reader.GetFieldValue<FollowRequestStatus>(2),
                Id = new FollowRequestId
                {
                    ReceiverId = reader.GetGuid(3),
                    RequesterId = reader.GetGuid(4)
                },
            };

        /// <summary>
        ///     Map a follow request to the context.
        /// </summary>
        /// <param name="context">The context to map to.</param>
        /// <param name="entity">The entity to map from.</param>
        private static void MapToWriter(DatabaseContext context, FollowRequest entity)
        {
            context.AddParameterWithValue("receiver_id", entity.Id.ReceiverId);
            context.AddParameterWithValue("requester_id", entity.Id.RequesterId);
        }
    }
}
