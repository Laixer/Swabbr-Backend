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
    ///     User database repository.
    /// </summary>
    internal class UserRepository : RepositoryBase, IUserRepository
    {
        // TODO Remove?
        public Task<Guid> CreateAsync(SwabbrUser entity) => throw new NotImplementedException();

        // TODO Remove?
        public Task DeleteAsync(Guid id) => throw new NotImplementedException();

        // TODO Do we ever need this due to forein key constraints?
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
                    WHERE   u.id = @id)";

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
                    WHERE   u.nickname = @nickname)";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("nickname", nickname);

            return await context.ScalarAsync<bool>();
        }

        /// <summary>
        ///     Gets a collection of users from our database.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Collection of users.</returns>
        public async IAsyncEnumerable<SwabbrUser> GetAllAsync(Navigation navigation)
        {
            var sql = @"
                    SELECT  u.birth_date,
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
                    FROM    application.user AS u";

            ConstructNavigation(ref sql, navigation);

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
        public async IAsyncEnumerable<SwabbrUser> GetAllVloggableUsersAsync(Navigation navigation)
        {
            var sql = @"
                    SELECT  u.birth_date,
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
                    FROM    application.user AS u
                    WHERE   u.vlog_request_limit > 0";

            ConstructNavigation(ref sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets a user from our database.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <returns>The user.</returns>
        public async Task<SwabbrUser> GetAsync(Guid id)
        {
            var sql = @"
                    SELECT  u.birth_date,
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
                    FROM    application.user AS u
                    WHERE   u.id = @id
                    LIMIT   1";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", id);

            await using var reader = await context.ReaderAsync();

            return MapFromReader(reader);
        }

        /// <summary>
        ///     Gets the followers of a given user.
        /// </summary>
        /// <param name="userId">The user that is being followed.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Followers of the specified user.</returns>
        public async IAsyncEnumerable<SwabbrUser> GetFollowersAsync(Guid userId, Navigation navigation)
        {
            var sql = @"
                    SELECT  u.birth_date,
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
                    FROM    application.user AS u
                    JOIN    application.follow_request AS fr
                    ON      fr.receiver_id = u.id
                    WHERE   u.id = @id
                    AND     fr.follow_request_status = 'accepted'";

            ConstructNavigation(ref sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", userId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
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
                    JOIN    application.follow_request AS fr
                    ON      fr.receiver_id = upnd.id
                    WHERE   u.id = @id
                    AND     fr.follow_request_status = 'accepted'";

            ConstructNavigation(ref sql, navigation);

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
        /// <param name="userId">The user that is following.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Users that the user is following.</returns>
        public async IAsyncEnumerable<SwabbrUser> GetFollowingAsync(Guid userId, Navigation navigation)
        {
            var sql = @"
                    SELECT  u.birth_date,
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
                    FROM    application.user AS u
                    JOIN    application.follow_request AS fr
                    ON      fr.requester_id = u.id
                    WHERE   u.id = @id
                    AND     fr.follow_request_status = 'accepted'";

            ConstructNavigation(ref sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", userId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets the push notification details for a
        ///     given user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>Push notification details.</returns>
        public async Task<UserPushNotificationDetails> GetPushDetailsAsync(Guid userId)
        {
            var sql = @"
                    SELECT  upnd.user_id,
                            upnd.push_notification_platform
                    FROM    application.user_push_notification_details AS upnd
                    WHERE   upnd.id = @id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", userId);

            await using var reader = await context.ReaderAsync();

            return MapPushNotificationDetailsFromReader(reader);
        }

        /// <summary>
        ///     Gets a user with its statistics.
        /// </summary>
        /// <param name="userId">The internal user id.</param>
        /// <returns>The user entity with statistics.</returns>
        public async Task<SwabbrUserWithStats> GetWithStatisticsAsync(Guid userId)
        {
            var sql = @"
                    SELECT  uws.birth_date,
                            uws.country,
                            uws.daily_vlog_request_limit,
                            uws.first_name,
                            uws.follow_mode,
                            uws.gender,
                            uws.id,
                            uws.is_private,
                            uws.last_name,
                            uws.latitude,
                            uws.longitude,
                            uws.nickname,
                            uws.profile_image_base64_encoded,
                            uws.timezone,
                            uws.total_followers,
                            uws.total_following,
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
        ///     Search for users in our data store.
        /// </summary>
        /// <param name="query">Search string.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>User search result set.</returns>
        public async IAsyncEnumerable<SwabbrUserWithStats> SearchAsync(string query, Navigation navigation)
        {
            var sql = @"
                    SELECT  uws.birth_date,
                            uws.country,
                            uws.daily_vlog_request_limit,
                            uws.first_name,
                            uws.follow_mode,
                            uws.gender,
                            uws.id,
                            uws.is_private,
                            uws.last_name,
                            uws.latitude,
                            uws.longitude,
                            uws.nickname,
                            uws.profile_image_base64_encoded,
                            uws.timezone,
                            uws.total_followers,
                            uws.total_following,
                            uws.total_vlogs
                    FROM    application.user_with_stats AS uws
                    WHERE   LOWER(uws.nickname) LIKE LOWER(@query%)";

            ConstructNavigation(ref sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("query", query);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapWithStatsFromReader(reader);
            }
        }

        // TODO Separate call for timezone and longlat, but timezone can be updated here as well. Consistency!
        /// <summary>
        ///     Update a user in our database.
        /// </summary>
        /// <remarks>
        ///     There exist separate calls to update the 
        ///     longitude and latitude properties.
        /// </remarks>
        /// <param name="entity">The user with updated properties.</param>
        public async Task UpdateAsync(SwabbrUser entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var sql = @"
                    UPDATE  application.user
                    SET     birth_date = @birth_date,
                            country = @country,
                            daily_vlog_request_limit = @daily_vlog_request_limit,
                            first_name = @first_name,
                            follow_mode = @follow_mode,
                            gender = @gender,
                            is_private = @is_private,
                            last_name = @last_name,
                            nickname = @nickname,
                            profile_image_base64_encoded = @profile_image_base64_encoded,
                            timezone = @timezone
                    WHERE   id = @id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", entity.Id);

            MapToWriter(context, entity);

            await context.NonQueryAsync();
        }

        // TODO Why separate call?
        public Task UpdateLocationAsync(Guid userId, double longitude, double latitude) => throw new NotImplementedException();

        // TODO Why separate call?
        public Task UpdateTimeZoneAsync(Guid userId, TimeZoneInfo newTimeZone) => throw new NotImplementedException();

        /// <summary>
        ///     Maps a reader to a user object.
        /// </summary>
        /// <param name="reader">The reader to map from.</param>
        /// <returns>The mapped user object.</returns>
        private static SwabbrUser MapFromReader(DbDataReader reader)
            => new SwabbrUser
            {
                BirthDate = reader.GetSafeDateTime(0),
                Country = reader.GetSafeString(1),
                DailyVlogRequestLimit = reader.GetUInt(2),
                FirstName = reader.GetSafeString(3),
                FollowMode = reader.GetFieldValue<FollowMode>(4),
                Gender = reader.GetFieldValue<Gender?>(5),
                Id = reader.GetGuid(6),
                IsPrivate = reader.GetBoolean(7),
                LastName = reader.GetSafeString(8),
                Latitude = reader.GetSafeFloat(9),
                Longitude = reader.GetSafeFloat(10),
                Nickname = reader.GetString(11),
                ProfileImageBase64Encoded = reader.GetSafeString(12),
                Timezone = reader.GetTimeZoneInfo(13)
            };

        /// <summary>
        ///     Maps a reader to a user with stats object.
        /// </summary>
        /// <param name="reader">The reader to map from.</param>
        /// <returns>The mapped user with stats object.</returns>
        private static SwabbrUserWithStats MapWithStatsFromReader(DbDataReader reader)
            => new SwabbrUserWithStats
            {
                BirthDate = reader.GetSafeDateTime(0),
                Country = reader.GetSafeString(1),
                DailyVlogRequestLimit = reader.GetUInt(2),
                FirstName = reader.GetSafeString(3),
                FollowMode = reader.GetFieldValue<FollowMode>(4),
                Gender = reader.GetFieldValue<Gender?>(5),
                Id = reader.GetGuid(6),
                IsPrivate = reader.GetBoolean(7),
                LastName = reader.GetSafeString(8),
                Latitude = reader.GetSafeFloat(9),
                Longitude = reader.GetSafeFloat(10),
                Nickname = reader.GetString(11),
                ProfileImageBase64Encoded = reader.GetSafeString(12),
                Timezone = reader.GetTimeZoneInfo(13),
                TotalFollowers = reader.GetUInt(14),
                TotalFollowing = reader.GetUInt(15),
                TotalVlogs = reader.GetUInt(16)
            };

        /// <summary>
        ///     Maps a reader to a push notification details object.
        /// </summary>
        /// <param name="reader">The reader to map from.</param>
        /// <returns>The mapped push notification details object.</returns>
        private static UserPushNotificationDetails MapPushNotificationDetailsFromReader(DbDataReader reader)
            =>  new UserPushNotificationDetails()
            {
                UserId = reader.GetGuid(0),
                PushNotificationPlatform = reader.GetFieldValue<PushNotificationPlatform>(1)
            };

        /// <summary>
        ///     Maps a swabbr user entity onto a writer.
        /// </summary>
        /// <param name="context">The context to add parameters to.</param>
        /// <param name="user">The user object.</param>
        private static void MapToWriter(DatabaseContext context, SwabbrUser user)
        {
            context.AddParameterWithValue("birth_date", user.BirthDate);
            context.AddParameterWithValue("country", user.Country);
            context.AddParameterWithValue("daily_vlog_request_limit", (int) user.DailyVlogRequestLimit); // Uint is not supported by postgresql.
            context.AddParameterWithValue("first_name", user.FirstName);
            context.AddParameterWithValue("follow_mode", user.FollowMode);
            context.AddParameterWithValue("gender", user.Gender);
            context.AddParameterWithValue("is_private", user.IsPrivate);
            context.AddParameterWithValue("last_name", user.LastName);
            context.AddParameterWithValue("nickname", user.Nickname);
            context.AddParameterWithValue("profile_image_base64_encoded", user.ProfileImageBase64Encoded);
            context.AddParameterWithValue("timezone", MapTimeZoneToString(user.Timezone));
        }

        // TODO Move to helper
        /// <summary>
        ///     Converts a timezone object to the expected
        ///     database format timezone.
        /// </summary>
        /// <param name="timeZoneInfo">The timezone object.</param>
        /// <returns>Formatted string.</returns>
        private static string MapTimeZoneToString(TimeZoneInfo timeZoneInfo)
        {
            var offset = timeZoneInfo.BaseUtcOffset;

            var hours = (offset.Hours >= 10) ? $"{offset.Hours}" : $"0{offset.Hours}";
            var minutes = (offset.Minutes >= 10) ? $"{offset.Minutes}" : $"0{offset.Minutes}";
            
            return $"UTC+{hours}:{minutes}";
        }
    }
}
