using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Helpers;
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
    ///     User database repository.
    /// </summary>
    internal class UserRepository : DatabaseContextBase, IUserRepository
    {
        /// <summary>
        ///     Checks if a user exists in our data store.
        /// </summary>
        /// <param name="id">The user id.</param>
        public async Task<bool> ExistsAsync(Guid id)
        {
            var sql = @"
                    SELECT  EXISTS (
                        SELECT  1
                        FROM    application.user AS u
                        WHERE   u.id = @id
                    )";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", id);

            return await context.ScalarAsync<bool>();
        }

        /// <summary>
        ///     Checks if a nickname already exists.
        /// </summary>
        /// <param name="nickname">The nickname to check for.</param>
        public async Task<bool> ExistsNicknameAsync(string nickname)
        {
            var sql = @"
                    SELECT  EXISTS (
                        SELECT  1
                        FROM    application.user AS u
                        WHERE   u.nickname = @nickname
                    )";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("nickname", nickname);

            return await context.ScalarAsync<bool>();
        }

        /// <summary>
        ///     Gets a collection of users from our database.
        /// </summary>
        /// <remarks>
        ///     This can order by <see cref="User.Nickname"/>.
        /// </remarks>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Collection of users.</returns>
        public async IAsyncEnumerable<User> GetAllAsync(Navigation navigation)
        {
            var sql = @"
                    SELECT  u.birth_date,
                            u.country,
                            u.daily_vlog_request_limit,
                            u.first_name,
                            u.follow_mode,
                            u.gender,
                            u.has_profile_image,
                            u.id,
                            u.is_private,
                            u.last_name,
                            u.latitude,
                            u.longitude,
                            u.nickname,
                            u.timezone
                    FROM    application.user_generic AS u";

            sql = ConstructNavigation(sql, navigation, "u.nickname");

            await using var context = await CreateNewDatabaseContext(sql);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets a collection of all users that are eligible
        ///     for a vlog request.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vloggable users.</returns>
        public async IAsyncEnumerable<User> GetAllVloggableUsersAsync(Navigation navigation)
        {
            var sql = @"
                    SELECT  u.birth_date,
                            u.country,
                            u.daily_vlog_request_limit,
                            u.first_name,
                            u.follow_mode,
                            u.gender,
                            u.has_profile_image,
                            u.id,
                            u.is_private,
                            u.last_name,
                            u.latitude,
                            u.longitude,
                            u.nickname,
                            u.timezone
                    FROM    application.user_generic AS u
                    WHERE   u.daily_vlog_request_limit > 0";

            sql = ConstructNavigation(sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        // TODO Add interests to all.
        /// <summary>
        ///     Gets a user from our database, including interests.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <returns>The user.</returns>
        public async Task<User> GetAsync(Guid id)
        {
            var sql = @"
                    SELECT      u.birth_date,
                                u.country,
                                u.daily_vlog_request_limit,
                                u.first_name,
                                u.follow_mode,
                                u.gender,
                                u.has_profile_image,
                                u.id,
                                u.is_private,
                                u.last_name,
                                u.latitude,
                                u.longitude,
                                u.nickname,
                                u.timezone,
                            
                                -- interests
                                ui.interest_1,                  
                                ui.interest_2,                  
                                ui.interest_3        

                    FROM        application.user_generic AS u
                    LEFT JOIN   application.user_interests AS ui
                    ON          u.id = ui.user_id
                    WHERE       u.id = @id
                    LIMIT       1";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", id);

            await using var reader = await context.ReaderAsync();

            var user = MapFromReader(reader);
            
            // Also the interests.
            user.Interest1 = reader.GetSafeString(14);
            user.Interest2 = reader.GetSafeString(15);
            user.Interest3 = reader.GetSafeString(16);

            return user;
        }

        /// <summary>
        ///     Gets the followers of a given user.
        /// </summary>
        /// <remarks>
        ///     This can order by <see cref="User.Nickname"/>.
        /// </remarks>
        /// <param name="userId">The user that is being followed.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Followers of the specified user.</returns>
        public async IAsyncEnumerable<User> GetFollowersAsync(Guid userId, Navigation navigation)
        {
            var sql = @"
                    SELECT  u.birth_date,
                            u.country,
                            u.daily_vlog_request_limit,
                            u.first_name,
                            u.follow_mode,
                            u.gender,
                            u.has_profile_image,
                            u.id,
                            u.is_private,
                            u.last_name,
                            u.latitude,
                            u.longitude,
                            u.nickname,
                            u.timezone
                    FROM    application.user_generic AS u
                    JOIN    application.follow_request_accepted AS fra
                    ON      fra.requester_id = u.id
                    WHERE   fra.receiver_id = @id";

            sql = ConstructNavigation(sql, navigation, "u.nickname");

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", userId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets all <see cref="UserWithRelationWrapper"/> objects that 
        ///     belong to the users which have pending follow requests for 
        ///     the current user.
        /// </summary>
        /// <param name="navigation">Result set control.</param>
        /// <returns>Wrappers around all users that liked saids vlogs.</returns>
        public async IAsyncEnumerable<UserWithRelationWrapper> GetFollowRequestingUsersAsync(Navigation navigation)
        {
            if (!AppContext.HasUser)
            {
                throw new InvalidOperationException();
            }

            var sql = @"
                SELECT  u.birth_date,
                        u.country,
                        u.daily_vlog_request_limit,
                        u.first_name,
                        u.follow_mode,
                        u.gender,
                        u.has_profile_image,
                        u.id,
                        u.is_private,
                        u.last_name,
                        u.latitude,
                        u.longitude,
                        u.nickname,
                        u.timezone
                FROM    application.user_generic AS u
                JOIN    application.follow_request AS fr
                ON      fr.requester_id = u.id
                WHERE   fr.follow_request_status = 'pending'
                        AND
                        fr.receiver_id = @user_id";

            sql = ConstructNavigation(sql, navigation, "fr.date_created");

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", AppContext.UserId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return new UserWithRelationWrapper
                {
                    RequestingUserId = AppContext.UserId,
                    FollowRequestStatus = FollowRequestStatus.Pending,
                    User = MapFromReader(reader)
                };
            }
        }

        /// <summary>
        ///     Gets the push notification details of the 
        ///     followers for a given user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Followers push notification details.</returns>
        public async IAsyncEnumerable<UserPushNotificationDetails> GetFollowersPushDetailsAsync(Guid userId, Navigation navigation)
        {
            var sql = @"
                    SELECT  upnd.user_id,
                            upnd.push_notification_platform
                    FROM    application.user_push_notification_details AS upnd
                    JOIN    application.follow_request_accepted AS fra
                    ON      fra.requester_id = upnd.user_id
                    WHERE   fra.receiver_id = @id";

            sql = ConstructNavigation(sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", userId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapPushNotificationDetailsFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets users that are being followed by a given user.
        /// </summary>
        /// <remarks>
        ///     This can order by <see cref="User.Nickname"/>.
        /// </remarks>
        /// <param name="userId">The user that is following.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Users that the user is following.</returns>
        public async IAsyncEnumerable<User> GetFollowingAsync(Guid userId, Navigation navigation)
        {
            var sql = @"
                    SELECT  u.birth_date,
                            u.country,
                            u.daily_vlog_request_limit,
                            u.first_name,
                            u.follow_mode,
                            u.gender,
                            u.has_profile_image,
                            u.id,
                            u.is_private,
                            u.last_name,
                            u.latitude,
                            u.longitude,
                            u.nickname,
                            u.timezone
                    FROM    application.user_generic AS u
                    JOIN    application.follow_request_accepted AS fra
                    ON      fra.receiver_id = u.id
                    WHERE   fra.requester_id = @id";

            sql = ConstructNavigation(sql, navigation, "u.nickname");

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", userId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets the push notification details for a given user.
        /// </summary>
        /// <remarks>
        ///     This throws if we have no push details for the user.
        /// </remarks>
        /// <param name="userId">The user id.</param>
        /// <returns>Push notification details.</returns>
        public async Task<UserPushNotificationDetails> GetPushDetailsAsync(Guid userId)
        {
            var sql = @"
                    SELECT  upnd.user_id,
                            upnd.push_notification_platform
                    FROM    application.user_push_notification_details AS upnd
                    WHERE   upnd.user_id = @id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", userId);

            await using var reader = await context.ReaderAsync();

            return MapPushNotificationDetailsFromReader(reader);
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
                SELECT  vlog_owner_id,

                        -- Follow request metadata
                        follow_request_status_or_null,

                        -- Vlog like, alphabetic properties
                        vlog_like_date_created,
                        vlog_like_user_id,
                        vlog_id,

                        -- Vlog liking user, alphabetic properties
                        user_birth_date,
                        user_country,
                        user_daily_vlog_request_limit,
                        user_first_name,
                        user_follow_mode,
                        user_gender,
                        user_has_profile_image,
                        user_id,
                        user_is_private,
                        user_last_name,
                        user_latitude,
                        user_longitude,
                        user_nickname,
                        user_timezone
                FROM    entities.vlog_liking_user
                WHERE   vlog_owner_id = @user_id";

            sql = ConstructNavigation(sql, navigation, "vlog_like_date_created");

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", AppContext.UserId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return new VlogLikingUserWrapper
                {
                    RequestingUserId = reader.GetGuid(0),
                    FollowRequestStatus = reader.GetFieldValue<FollowRequestStatus?>(1),
                    VlogLike = VlogLikeRepository.MapFromReader(reader, 2),
                    User = MapFromReader(reader, 5)
                };
            }
        }

        /// <summary>
        ///     Gets a user with its statistics.
        /// </summary>
        /// <param name="userId">The internal user id.</param>
        /// <returns>The user entity with statistics.</returns>
        public async Task<UserWithStats> GetWithStatisticsAsync(Guid userId)
        {
            var sql = @"
                    SELECT  -- User
                            uws.birth_date,
                            uws.country,
                            uws.daily_vlog_request_limit,
                            uws.first_name,
                            uws.follow_mode,
                            uws.gender,
                            uws.has_profile_image,
                            uws.id,
                            uws.is_private,
                            uws.last_name,
                            uws.latitude,
                            uws.longitude,
                            uws.nickname,
                            uws.timezone,
                            
                            -- Statistics
                            uws.total_followers,
                            uws.total_following,
                            uws.total_likes_received,
                            uws.total_reactions_given,
                            uws.total_reactions_received,
                            uws.total_views,
                            uws.total_vlogs
                    FROM    application.user_with_stats AS uws
                    WHERE   uws.id = @id
                    LIMIT   1";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", userId);

            await using var reader = await context.ReaderAsync();

            return MapWithStatsFromReader(reader);
        }

        /// <summary>
        ///     Search for users in our data store, also checks for interests.
        /// </summary>
        /// <remarks>
        ///     This can order by <see cref="User.Nickname"/>.
        /// </remarks>
        /// <param name="query">Search string.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>User search result set.</returns>
        public async IAsyncEnumerable<UserWithRelationWrapper> SearchAsync(string query, Navigation navigation)
        {
            if (!AppContext.HasUser)
            {
                throw new InvalidOperationException();
            }

            var sql = @"
                WITH results AS (
	                SELECT		u.birth_date,
				                u.country,
				                u.daily_vlog_request_limit,
				                u.first_name,
				                u.follow_mode,
				                u.gender,
				                u.has_profile_image,
				                u.id,
				                u.is_private,
				                u.last_name,
				                u.latitude,
				                u.longitude,
				                u.nickname,
				                u.timezone
	                FROM		application.user_generic AS u
	                LEFT JOIN 	application.user_interests AS ui 
	                ON			u.id = ui.user_id
	                WHERE		LOWER(u.nickname) LIKE LOWER(@query)
				                OR 
				                LOWER(ui.interest_1) LIKE LOWER(@query)
				                OR 
				                LOWER(ui.interest_2) LIKE LOWER(@query)
				                OR 
				                LOWER(ui.interest_3) LIKE LOWER(@query)
                )
                SELECT		r.birth_date,
			                r.country,
			                r.daily_vlog_request_limit,
			                r.first_name,
			                r.follow_mode,
			                r.gender,
			                r.has_profile_image,
			                r.id,
			                r.is_private,
			                r.last_name,
			                r.latitude,
			                r.longitude,
			                r.nickname,
			                r.timezone,
			                COALESCE(fr.follow_request_status) AS follow_request_status_or_null
                FROM 		results AS r
                LEFT JOIN	application.follow_request AS fr
                ON			fr.requester_id = @self_user_id
			                AND
			                fr.receiver_id = r.id";

            sql = ConstructNavigation(sql, navigation, "r.nickname");

            await using var context = await CreateNewDatabaseContext(sql);

            // Manually append the % wildcard
            context.AddParameterWithValue("query", $"{query}%");
            context.AddParameterWithValue("self_user_id", AppContext.UserId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return new UserWithRelationWrapper {
                    RequestingUserId = AppContext.UserId,
                    User = MapFromReader(reader, 0),
                    FollowRequestStatus = reader.GetFieldValue<FollowRequestStatus?>(14),
                };
            }
        }

        /// <summary>
        ///     Update the current user in our database.
        /// </summary>
        /// <remarks>
        ///     There exist separate calls to update the 
        ///     longitude and latitude properties.
        /// </remarks>
        /// <param name="entity">The user with updated properties.</param>
        public async Task UpdateAsync(User entity)
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
                    UPDATE  application.user
                    SET     birth_date = @birth_date,
                            country = @country,
                            daily_vlog_request_limit = @daily_vlog_request_limit,
                            first_name = @first_name,
                            follow_mode = @follow_mode,
                            gender = @gender,
                            has_profile_image = @has_profile_image,
                            is_private = @is_private,
                            last_name = @last_name,
                            latitude = @latitude,
                            longitude = @longitude,
                            nickname = @nickname,
                            timezone = @timezone
                    WHERE   id = @id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", AppContext.UserId);

            MapToWriter(context, entity);

            await context.NonQueryAsync();

            // Also update the interests.
            await UpdateInterestsAsync(entity);
        }

        /// <summary>
        ///     Update the current users interests in our database.
        /// </summary>
        /// <param name="entity">The user with updated properties.</param>
        private async Task UpdateInterestsAsync(User entity)
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
                    INSERT INTO application.user_interests (
                        user_id,
                        interest_1,
                        interest_2,
                        interest_3
                    ) VALUES (
                        @user_id,
                        @interest_1,
                        @interest_2,
                        @interest_3
                    )
                    ON CONFLICT (user_id)
                    DO UPDATE SET
                        interest_1 = @interest_1,
                        interest_2 = @interest_2,
                        interest_3 = @interest_3";
                    
            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", AppContext.UserId);
            context.AddParameterWithValue("interest_1", entity.Interest1);
            context.AddParameterWithValue("interest_2", entity.Interest2);
            context.AddParameterWithValue("interest_3", entity.Interest3);

            await context.NonQueryAsync();
        }

        /// <summary>
        ///     Maps a reader to a user object.
        /// </summary>
        /// <param name="reader">The reader to map from.</param>
        /// <param name="offset">Ordinal offset.</param>
        /// <returns>The mapped user object.</returns>
        internal static User MapFromReader(DbDataReader reader, int offset = 0)
            => new User
            {
                BirthDate = reader.GetSafeDateTime(0 + offset),
                Country = reader.GetSafeString(1 + offset),
                DailyVlogRequestLimit = reader.GetUInt(2 + offset),
                FirstName = reader.GetSafeString(3 + offset),
                FollowMode = reader.GetFieldValue<FollowMode>(4 + offset),
                Gender = reader.GetFieldValue<Gender?>(5 + offset),
                HasProfileImage = reader.GetBoolean(6 + offset),
                Id = reader.GetGuid(7 + offset),
                IsPrivate = reader.GetBoolean(8 + offset),
                LastName = reader.GetSafeString(9 + offset),
                Latitude = reader.GetSafeDouble(10 + offset),
                Longitude = reader.GetSafeDouble(11 + offset),
                Nickname = reader.GetString(12 + offset),
                TimeZone = reader.GetTimeZoneInfo(13 + offset)
            };

        /// <summary>
        ///     Maps a reader to a user with stats object.
        /// </summary>
        /// <param name="reader">The reader to map from.</param>
        /// <param name="offset">Ordinal offset.</param>
        /// <returns>The mapped user with stats object.</returns>
        internal static UserWithStats MapWithStatsFromReader(DbDataReader reader, int offset = 0)
            => new UserWithStats
            {
                BirthDate = reader.GetSafeDateTime(0 + offset),
                Country = reader.GetSafeString(1 + offset),
                DailyVlogRequestLimit = reader.GetUInt(2 + offset),
                FirstName = reader.GetSafeString(3 + offset),
                FollowMode = reader.GetFieldValue<FollowMode>(4 + offset),
                Gender = reader.GetFieldValue<Gender?>(5 + offset),
                HasProfileImage = reader.GetBoolean(6 + offset),
                Id = reader.GetGuid(7 + offset),
                IsPrivate = reader.GetBoolean(8 + offset),
                LastName = reader.GetSafeString(9 + offset),
                Latitude = reader.GetSafeDouble(10 + offset),
                Longitude = reader.GetSafeDouble(11 + offset),
                Nickname = reader.GetString(12 + offset),
                TimeZone = reader.GetTimeZoneInfo(13 + offset),
                TotalFollowers = reader.GetUInt(14 + offset),
                TotalFollowing = reader.GetUInt(15 + offset),
                TotalLikesReceived = reader.GetUInt(16 + offset),
                TotalReactionsGiven = reader.GetUInt(17 + offset),
                TotalReactionsReceived = reader.GetUInt(18 + offset),
                TotalViews = reader.GetUInt(19 + offset),
                TotalVlogs = reader.GetUInt(20 + offset)
            };

        /// <summary>
        ///     Maps a reader to a push notification details object.
        /// </summary>
        /// <param name="reader">The reader to map from.</param>
        /// <param name="offset">Ordinal offset.</param>
        /// <returns>The mapped push notification details object.</returns>
        internal static UserPushNotificationDetails MapPushNotificationDetailsFromReader(DbDataReader reader, int offset = 0)
            =>  new UserPushNotificationDetails()
            {
                UserId = reader.GetGuid(0 + offset),
                PushNotificationPlatform = reader.GetFieldValue<PushNotificationPlatform>(1 + offset)
            };

        /// <summary>
        ///     Maps a swabbr user entity onto a writer.
        /// </summary>
        /// <param name="context">The context to add parameters to.</param>
        /// <param name="user">The user object.</param>
        private static void MapToWriter(DatabaseContext context, User user)
        {
            context.AddParameterWithValue("birth_date", user.BirthDate);
            context.AddParameterWithValue("country", user.Country);
            context.AddParameterWithValue("daily_vlog_request_limit", (int) user.DailyVlogRequestLimit);
            context.AddParameterWithValue("first_name", user.FirstName);
            context.AddParameterWithValue("follow_mode", user.FollowMode);
            context.AddParameterWithValue("gender", user.Gender);
            context.AddParameterWithValue("has_profile_image", user.HasProfileImage);
            context.AddParameterWithValue("is_private", user.IsPrivate);
            context.AddParameterWithValue("last_name", user.LastName);
            context.AddParameterWithValue("latitude", user.Latitude);
            context.AddParameterWithValue("longitude", user.Longitude);
            context.AddParameterWithValue("nickname", user.Nickname);
            context.AddParameterWithValue("timezone", TimeZoneInfoHelper.MapTimeZoneToStringOrNull(user.TimeZone));
        }
    }
}
