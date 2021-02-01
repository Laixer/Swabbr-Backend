using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
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
    // TODO This does not take non-existing users into account.
    // TODO The order by using create_date will not work for follow requests which were declined and then re-sent.
    /// <summary>
    ///     Repository for follow requests.
    /// </summary>
    internal class FollowRequestRepository : DatabaseContextBase, IFollowRequestRepository
    {
        /// <summary>
        ///     Create a new follow request.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This expects the requester id to be equal to the current 
        ///         user id. A <see cref="NotAllowedException"/> is thrown
        ///         otherwise.
        ///     </para>
        ///     <para>
        ///         This doesn't actually return the created entity
        ///         id since we already now said id.
        ///     </para>
        /// </remarks>
        /// <param name="entity">The follow request.</param>
        /// <returns>The created follow requests id.</returns>
        public async Task<FollowRequestId> CreateAsync(FollowRequest entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (entity.Id is null)
            {
                throw new ArgumentNullException(nameof(entity.Id));
            }
            
            // Only create the follow request if we are the requester
            if (!AppContext.HasUser || entity.Id.RequesterId != AppContext.UserId)
            {
                throw new NotAllowedException();
            }

            var sql = @"
                    INSERT INTO application.follow_request(
                        receiver_id,
                        requester_id
                    )
                    VALUES (
                        @receiver_id,
                        @requester_id
                    )";

            await using var context = await CreateNewDatabaseContext(sql);

            MapToWriter(context, entity);

            await context.NonQueryAsync();

            return entity.Id;
        }

        /// <summary>
        ///     Deletes a follow request from our data store.
        /// </summary>
        /// <remarks>
        ///     An <see cref="NotAllowedException"/> is thrown 
        ///     if the current user id does not equal either 
        ///     the requester id or receiver id.
        /// </remarks>
        /// <param name="id">Follow request id.</param>
        public async Task DeleteAsync(FollowRequestId id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (!AppContext.HasUser || (!AppContext.IsUser(id.RequesterId) && !AppContext.IsUser(id.ReceiverId)))
            {
                throw new NotAllowedException();
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
                        AND     fr.requester_id = @requester_id
                    )";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("receiver_id", id.ReceiverId);
            context.AddParameterWithValue("requester_id", id.RequesterId);

            return await context.ScalarAsync<bool>();
        }

        /// <summary>
        ///     Gets all follow requests from our database.
        /// </summary>
        /// <remarks>
        ///     This can order by <see cref="FollowRequest.DateCreated"/>.
        /// </remarks>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Follow request result set.</returns>
        public async IAsyncEnumerable<FollowRequest> GetAllAsync(Navigation navigation)
        {
            var sql = @"
                    SELECT  fr.date_created,
                            fr.date_updated,
                            fr.follow_request_status,
                            fr.receiver_id,
                            fr.requester_id
                    FROM    application.follow_request AS fr";

            sql = ConstructNavigation(sql, navigation, "fr.date_created");

            await using var context = await CreateNewDatabaseContext(sql);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }
        
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
                    SELECT  COUNT(fra.*)
                    FROM    application.follow_request_accepted AS fra
                    WHERE   fra.receiver_id = @receiver_id";

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
                    SELECT  COUNT(fra.*)
                    FROM    application.follow_request_accepted AS fra
                    WHERE   fra.requester_id = @requester_id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("requester_id", userId);

            return (uint)await context.ScalarAsync<int>();
        }

        /// <summary>
        ///     Gets incoming follow requests for a user.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The current user id is extracted from the app context.
        ///     </para>
        ///     <para>
        ///         This can order by <see cref="FollowRequest.DateCreated"/>.
        ///     </para>
        /// </remarks>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Incoming follow requests.</returns>
        public async IAsyncEnumerable<FollowRequest> GetIncomingForUserAsync(Navigation navigation)
        {
            if (!AppContext.HasUser)
            {
                throw new NotAllowedException();
            }

            var sql =@"
                    SELECT  fr.date_created,
                            fr.date_updated,
                            fr.follow_request_status,
                            fr.receiver_id,
                            fr.requester_id
                    FROM    application.follow_request AS fr
                    WHERE   fr.receiver_id = @receiver_id
                    AND     fr.follow_request_status = 'pending'" ;

            sql = ConstructNavigation(sql, navigation, "fr.date_created");

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("receiver_id", AppContext.UserId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets outgoing follow requests for a user.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The current user id is extracted from the app context.
        ///     </para>
        ///     <para>
        ///         This can order by <see cref="FollowRequest.DateCreated"/>.
        ///     </para>
        /// </remarks>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Outgoing follow requests.</returns>
        public async IAsyncEnumerable<FollowRequest> GetOutgoingForUserAsync(Navigation navigation)
        {
            if (!AppContext.HasUser)
            {
                throw new NotAllowedException();
            }

            var sql = @"
                    SELECT  fr.date_created,
                            fr.date_updated,
                            fr.follow_request_status,
                            fr.receiver_id,
                            fr.requester_id
                    FROM    application.follow_request AS fr
                    WHERE   fr.requester_id = @requester_id
                    AND     fr.follow_request_status = 'pending'";

            sql = ConstructNavigation(sql, navigation, "fr.date_created");

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("requester_id", AppContext.UserId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Updates the status of a follow request.
        /// </summary>
        /// <remarks>
        ///     This throws a <see cref="NotAllowedException"/>
        ///     if the current user id is not equal to either 
        ///     the requester or the receiver id.
        /// </remarks>
        /// <param name="id">The follow request id.</param>
        /// <param name="status">The new status.</param>
        public async Task UpdateStatusAsync(FollowRequestId id, FollowRequestStatus status)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (!AppContext.HasUser || (!AppContext.IsUser(id.RequesterId) && !AppContext.IsUser(id.ReceiverId)))
            {
                throw new NotAllowedException();
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
        /// <param name="offset">Ordinal offset.</param>
        /// <returns>The mapped follow request.</returns>
        internal static FollowRequest MapFromReader(DbDataReader reader, int offset = 0)
            => new FollowRequest
            {
                DateCreated = reader.GetDateTime(0 + offset),
                DateUpdated = reader.GetSafeDateTime(1 + offset),
                FollowRequestStatus = reader.GetFieldValue<FollowRequestStatus>(2 + offset),
                Id = new FollowRequestId
                {
                    ReceiverId = reader.GetGuid(3 + offset),
                    RequesterId = reader.GetGuid(4 + offset)
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
