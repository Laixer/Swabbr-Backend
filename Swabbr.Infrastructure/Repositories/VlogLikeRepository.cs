using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
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
    ///     Repository for vlog likes.
    /// </summary>
    /// <remarks>
    ///     This repository checks vlog_like_nondeleted, not vlog_like, 
    ///     meaning any deleted vlogs make their vlog likes seem deleted 
    ///     as well. The entries in the database remain, but these methods
    ///     will perform as if the entries don't exist.
    /// </remarks>
    internal class VlogLikeRepository : DatabaseContextBase, IVlogLikeRepository
    {
        /// <summary>
        ///     Create a new vlog like in the database.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The vloglike user id is extracted from the context.
        ///     </para>
        ///     <para>
        ///         This returns the existing id property of
        ///         <paramref name="entity"/> since this is 
        ///         used as primary key in the database.
        ///     </para>
        /// </remarks>
        /// <param name="entity">The vlog like to create.</param>
        /// <returns>The created vlog like id.</returns>
        public async Task<VlogLikeId> CreateAsync(VlogLike entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (!AppContext.HasUser)
            {
                throw new NotAllowedException();
            }

            var sql = @"
                    INSERT INTO entities.vlog_like (
                        user_id,
                        vlog_id
                    )
                    VALUES (
                        @user_id,
                        @vlog_id
                    )";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", AppContext.UserId);
            context.AddParameterWithValue("vlog_id", entity.Id.VlogId);

            await context.NonQueryAsync();

            // We already know the id.
            return entity.Id;
        }

        /// <summary>
        ///     Deletes a vlog like in our database.
        /// </summary>
        /// <remarks>
        ///     This expects the current user to own the vlog.
        /// </remarks>
        /// <param name="id">The vlog like id.</param>
        public async Task DeleteAsync(VlogLikeId id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (!AppContext.HasUser || !AppContext.IsUser(id.UserId))
            {
                throw new NotAllowedException();
            }

            var sql = @"
                    DELETE 
                    FROM    entities.vlog_like AS vl
                    WHERE   vl.user_id = @user_id
                    AND     vl.vlog_id = @vlog_id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", id.UserId);
            context.AddParameterWithValue("vlog_id", id.VlogId);

            await context.NonQueryAsync();
        }

        /// <summary>
        ///     Checks if a vlog like with given id exists.
        /// </summary>
        /// <param name="id">The id to check for.</param>
        public async Task<bool> ExistsAsync(VlogLikeId id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var sql = @"
                    SELECT  EXISTS (
                        SELECT  1
                        FROM    entities.vlog_like_nondeleted AS vl
                        WHERE   vl.user_id = @user_id
                        AND     vl.vlog_id = @vlog_id
                    )";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", id.UserId);
            context.AddParameterWithValue("vlog_id", id.VlogId);

            return await context.ScalarAsync<bool>();
        }

        /// <summary>
        ///     Gets all vlog likes from our database.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlog like return set.</returns>
        public async IAsyncEnumerable<VlogLike> GetAllAsync(Navigation navigation)
        {
            var sql = @"
                    SELECT  vl.date_created,
                            vl.user_id,
                            vl.vlog_id
                    FROM    entities.vlog_like_nondeleted AS vl";

            sql = ConstructNavigation(sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets a vlog like from our database.
        /// </summary>
        /// <param name="id">The vlog like id.</param>
        /// <returns>The vlog like.</returns>
        public async Task<VlogLike> GetAsync(VlogLikeId id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var sql = @"
                    SELECT  vl.date_created,
                            vl.user_id,
                            vl.vlog_id
                    FROM    entities.vlog_like_nondeleted AS vl
                    WHERE   vl.vlog_id = @vlog_id
                            AND
                            vl.user_id = @user_id
                    LIMIT   1";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", id.UserId);
            context.AddParameterWithValue("vlog_id", id.VlogId);

            await using var reader = await context.ReaderAsync();

            return MapFromReader(reader);
        }

        /// <summary>
        ///     Gets all vlog likes for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to get likes for.</param>
        /// <param name="navigation">Navigation control./param>
        /// <returns>Vlog likes for the vlog.</returns>
        public async IAsyncEnumerable<VlogLike> GetForVlogAsync(Guid vlogId, Navigation navigation)
        {
            var sql = @"
                    SELECT  vl.date_created,
                            vl.user_id,
                            vl.vlog_id
                    FROM    entities.vlog_like_nondeleted AS vl
                    WHERE   vl.vlog_id = @vlog_id";

            sql = ConstructNavigation(sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("vlog_id", vlogId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets a vlog like summary for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to summarize.</param>
        /// <returns>The vlog like summary.</returns>
        public async Task<VlogLikeSummary> GetSummaryForVlogAsync(Guid vlogId)
        {
            var sql = @"
                WITH cnt AS (
                    SELECT  count(vl.vlog_id) AS count
                    FROM    entities.vlog_like_nondeleted AS vl
                    WHERE   vl.vlog_id = @vlog_id
                )
                SELECT      cnt.count,
		                    u.birth_date,
                            u.country,
                            u.daily_vlog_request_limit,
                            u.first_name,
                            u.follow_mode,
                            u.gender,
                            u.id,
                            u.is_private,
                            u.last_name,
                            u.latitude,
                            u.longitude,
                            u.nickname,
                            u.profile_image_base64_encoded,
                            u.timezone
                FROM        cnt AS cnt
                LEFT JOIN (
                    SELECT      vl.user_id,
    		                    vl.vlog_id
                    FROM        entities.vlog_like_nondeleted AS vl
                    WHERE       vl.vlog_id = @vlog_id
                    LIMIT       5
                )
                AS      	vlog_likes
                ON 			vlog_likes.vlog_id = @vlog_id
                LEFT JOIN   application.user AS u 
                ON      	vlog_likes.user_id = u.id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("vlog_id", vlogId);

            await using var reader = await context.ReaderAsync();

            // If no vlog likes exist, return an empty summary
            var count = reader.GetUInt(0);
            if (count == 0)
            {
                return EmptyVlogLikeSummary(vlogId);
            }

            // Else, extract each user.
            var users = new List<User>();
            do
            {
                // Pass an offset of 1 since the first column is the count.
                users.Add(UserRepository.MapFromReader(reader, 1));
            }
            while (await reader.NextResultAsync());

            // Compose and return.
            return new VlogLikeSummary
            {
                TotalLikes = count,
                Users = users,
                VlogId = vlogId
            };
        }

        /// <summary>
        ///     Gets all <see cref="VlogLikingUserWrapper"/> objects that 
        ///     belong to the vlogs of a given <paramref name="userId"/>.
        /// </summary>
        /// <param name="navigation">Result set control.</param>
        /// <returns>Wrappers around all users that liked saids vlogs.</returns>
        public async IAsyncEnumerable<VlogLikingUserWrapper> GetVlogLikingUsersForUserAsync(Navigation navigation)
        {
            if (!AppContext.HasUser)
            {
                throw new InvalidOperationException();
            }
            
            var sql = @"
                SELECT
                    vlog_owner_id,

                    -- Follow request metadata
                    follow_request_status_or_null,

                    -- Vlog like, alphabetic properties
                    vlog_like_date_created,
                    vlog_id,
                    vlog_like_user_id,

                    -- Vlog liking user, alphabetic properties
                    user_birth_date,
                    user_country,
                    user_daily_vlog_request_limit,
                    user_first_name,
                    user_follow_mode,
                    user_gender,
                    user_id,
                    user_is_private,
                    user_last_name,
                    user_latitude,
                    user_longitude,
                    user_nickname,
                    user_profile_image_base64_encoded,
                    user_timezone
                FROM
                    entities.vlog_liking_user
                WHERE
                    vlog_owner_id = @user_id";

            sql = ConstructNavigation(sql, navigation, "vlog_like_date_created");

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", AppContext.UserId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return new VlogLikingUserWrapper
                {
                    RequestingUserId = reader.GetGuid(0),
                    FollowRequestStatus = reader.GetFieldValue<FollowRequestStatus?>(1),
                    VlogLike = MapFromReader(reader, 2),
                    User = UserRepository.MapFromReader(reader, 5)
                };
            }
        }

        /// <summary>
        ///     Maps a vlog like from a reader.
        /// </summary>
        /// <param name="reader">The reader to map from.</param>
        /// <param name="offset">Ordinal offset.</param>
        /// <returns>The mapped vlog like.</returns>
        internal static VlogLike MapFromReader(DbDataReader reader, int offset = 0)
            => new VlogLike
            {
                DateCreated = reader.GetDateTime(0 + offset),
                Id = new VlogLikeId
                {
                    UserId = reader.GetGuid(1 + offset),
                    VlogId = reader.GetGuid(2 + offset)
                }
            };

        /// <summary>
        ///     Creates an empty vlog like summary.
        /// </summary>
        /// <param name="vlogId">The vlog that is being summarized.</param>
        /// <returns>Empty vlog like summary.</returns>
        private static VlogLikeSummary EmptyVlogLikeSummary(Guid vlogId)
            => new VlogLikeSummary
            {
                TotalLikes = 0,
                Users = new List<User>(),
                VlogId = vlogId
            };
    }
}
