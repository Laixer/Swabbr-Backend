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
        public Task<Guid> CreateAsync(SwabbrUser entity) => throw new NotImplementedException();
        public Task DeleteAsync(Guid id) => throw new NotImplementedException();
        public Task<bool> ExistsAsync(Guid id) => throw new NotImplementedException();

        /// <summary>
        ///     Checks if a nickname already exists.
        /// </summary>
        /// <param name="nickname">The nickname to check for.</param>
        public async Task<bool> ExistsNicknameAsync(string nickname)
        {
            // TODO Ugly format imo
            var sql = @"
                    SELECT  EXISTS (
                    SELECT  1
                    FROM    application.user
                    WHERE   nickname = @nickname)";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("nickname", nickname);

            return await context.ScalarAsync<bool>();
        }

        public IAsyncEnumerable<SwabbrUser> GetAllAsync(Navigation navigation) => throw new NotImplementedException();
        public IAsyncEnumerable<SwabbrUser> GetAllVloggableUsersAsync(Navigation navigation) => throw new NotImplementedException();

        /// <summary>
        ///     Gets a user from our database.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <returns>The user.</returns>
        public async Task<SwabbrUser> GetAsync(Guid id)
        {
            var sql = @"
                    SELECT  birth_date,
                            country,
                            daily_vlog_request_limit,
                            first_name,
                            follow_mode,
                            gender,
                            id,
                            is_private,
                            last_name,
                            latitude,
                            longitude,
                            nickname,
                            profile_image_base64_encoded,
                            timezone
                    FROM    application.user
                    WHERE   id = @id
                    LIMIT   1";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", id);

            await using var reader = await context.ReaderAsync();

            return MapFromReader(reader);
        }

        public IAsyncEnumerable<SwabbrUser> GetFollowersAsync(Guid userId, Navigation navigation) => throw new NotImplementedException();
        public IAsyncEnumerable<UserPushNotificationDetails> GetFollowersPushDetailsAsync(Guid userId, Navigation navigation) => throw new NotImplementedException();
        public IAsyncEnumerable<SwabbrUser> GetFollowingAsync(Guid userId, Navigation navigation) => throw new NotImplementedException();
        public Task<UserPushNotificationDetails> GetPushDetailsAsync(Guid userId) => throw new NotImplementedException();
        public Task<SwabbrUserWithStats> GetWithStatisticsAsync(Guid userId) => throw new NotImplementedException();
        public IAsyncEnumerable<SwabbrUserWithStats> SearchAsync(string query, Navigation navigation) => throw new NotImplementedException();

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
