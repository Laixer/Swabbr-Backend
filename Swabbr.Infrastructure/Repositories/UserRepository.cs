using Dapper;
using Laixer.Infra.Npgsql;
using Laixer.Utility.Exceptions;
using Laixer.Utility.Extensions;
using Microsoft.Extensions.Options;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using Swabbr.Core.Utility;
using Swabbr.Infrastructure.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using static Swabbr.Infrastructure.Database.DatabaseConstants;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="SwabbrUser"/> entities.
    /// </summary>
    public sealed class UserRepository : IUserRepository
    {

        private readonly IDatabaseProvider _databaseProvider;
        private readonly SwabbrConfiguration swabbrConfiguration;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public UserRepository(IDatabaseProvider databaseProvider,
            IOptions<SwabbrConfiguration> options)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            options.Value.ThrowIfInvalid();
            swabbrConfiguration = options.Value;
        }

        public Task<SwabbrUser> CreateAsync(SwabbrUser entity)
        {
            // TODO THOMAS This probably shouldn't even have this function.
            throw new InvalidOperationException("Creation of users should ONLY be done by the identity framework!");
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all <see cref="SwabbrUserMinified"/> from the database.
        /// </summary>
        /// <remarks>
        /// This ignores all users that have <see cref="SwabbrUser.DailyVlogRequestLimit"/>
        /// set to 0.
        /// </remarks>
        /// <returns><see cref="SwabbrUserMinified"/> colletion</returns>
        public async Task<IEnumerable<SwabbrUserMinified>> GetAllVloggableUserMinifiedAsync()
        {
            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    SELECT 
                        id, 
                        daily_vlog_request_limit AS DailyVlogRequestLimit,
                        timezone
                    FROM {TableUser} 
                    WHERE daily_vlog_request_limit > 0";
                return await connection.QueryAsync<SwabbrUserMinified>(sql).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets a single <see cref="SwabbrUser"/> from the database based on its internal id.
        /// </summary>
        /// <remarks>
        /// Throws an <see cref="EntityNotFoundException"/> if the entity doesn't exist.
        /// </remarks>
        /// <param name="userId">Internal id</param>
        /// <returns><see cref="SwabbrUser"/></returns>
        public async Task<SwabbrUser> GetAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();
            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"SELECT * FROM {TableUser} WHERE id = @Id FOR UPDATE;";
                var result = await connection.QueryAsync<SwabbrUser>(sql, new { Id = userId }).ConfigureAwait(false);
                if (result == null || !result.Any()) { throw new EntityNotFoundException($"Could not find User with id = {userId}"); }
                else
                {
                    return result.First();
                }
            }
        }

        /// <summary>
        /// Gets a single <see cref="SwabbrUser"/> based on its email.
        /// </summary>
        /// <remarks>
        /// Throws an <see cref="EntityNotFoundException"/> if the entity doesn't exist.
        /// </remarks>
        /// <param name="email">User email</param>
        /// <returns><see cref="SwabbrUser"/></returns>
        public async Task<SwabbrUser> GetByEmailAsync(string email)
        {
            email.ThrowIfNullOrEmpty();
            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"SELECT * FROM {TableUser} WHERE email = '{email}';";
                var result = await connection.QueryAsync<SwabbrUser>(sql).ConfigureAwait(false);
                if (result == null || !result.Any()) { throw new EntityNotFoundException($"Could not find User with email = {email}"); }
                else
                {
                    return result.First();
                }
            }
        }

        /// <summary>
        /// Gets all <see cref="SwabbrUser"/> entities that follow a given
        /// <see cref="SwabbrUser"/> specified by <paramref name="userId"/>.
        /// 
        /// TODO Pagination?
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns>All followers for <paramref name="userId"/></returns>
        public async Task<IEnumerable<SwabbrUser>> GetFollowersAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    SELECT * FROM {ViewUserWithStats} AS u
                    JOIN {TableFollowRequest} AS f
                    ON u.id = f.requester_id
                    WHERE f.receiver_id = @Id";
                return await connection.QueryAsync<SwabbrUserWithStats>(sql, new { Id = userId }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the <see cref="UserPushNotificationDetails"/> for all followers 
        /// of a specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="UserPushNotificationDetails"/> collection</returns>
        public async Task<IEnumerable<UserPushNotificationDetails>> GetFollowersPushDetailsAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            if (!await UserExistsAsync(userId).ConfigureAwait(false)) { throw new UserNotFoundException(); }

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    SELECT 
                        u.id AS UserId, 
                        nr.push_notification_platform AS PushNotificationPlatform
                    FROM {TableUser} AS u
                    JOIN {TableFollowRequest} AS fr
                    ON u.id = fr.requester_id
                    JOIN {TableNotificationRegistration} AS nr
                    ON u.id = nr.user_id
                    WHERE fr.receiver_id = @Id";
                return await connection.QueryAsync<UserPushNotificationDetails>(sql, new { Id = userId }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets all <see cref="SwabbrUser"/> entities that a given
        /// <see cref="SwabbrUser"/> specified by <paramref name="userId"/>
        /// is following.
        /// 
        /// TODO Pagination?
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns>All users that <paramref name="userId"/> follows</returns>
        public async Task<IEnumerable<SwabbrUser>> GetFollowingAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    SELECT * FROM {ViewUserWithStats} AS u
                    JOIN {TableFollowRequest} AS f
                    ON u.id = f.receiver_id
                    WHERE f.requester_id = @Id";
                return await connection.QueryAsync<SwabbrUserWithStats>(sql, new { Id = userId }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the <see cref="UserPushNotificationDetails"/> for a given 
        /// <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="UserPushNotificationDetails"/></returns>
        public async Task<UserPushNotificationDetails> GetPushDetailsAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();
            if (!await UserExistsAsync(userId).ConfigureAwait(false)) { throw new UserNotFoundException(); }

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                // TODO Mapping doesn't work for some reason
                var sql = $"SELECT push_notification_platform AS PushNotificationPlatform, user_id as UserId FROM {ViewUserPushNotificationDetails} WHERE user_id = @UserId";
                var result = await connection.QueryAsync<UserPushNotificationDetails>(sql, new { UserId = userId }).ConfigureAwait(false);
                if (result == null || !result.Any()) { throw new EntityNotFoundException(); }
                if (result.Count() > 1) { throw new InvalidOperationException("Found multiple entities on single get"); } // TODO Is this correct?
                return result.First();
            }
        }

        /// <summary>
        /// Gets the <see cref="SwabbrUser"/> to which a given <see cref="Vlog"/>
        /// belongs.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="SwabbrUser"/></returns>
        public async Task<SwabbrUser> GetUserFromVlogAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    SELECT u.* 
                    FROM {TableVlog} AS v
                    JOIN {TableUser} AS u
                    ON v.user_id = u.id
                    WHERE v.id = @VlogId
                    FOR UPDATE";
                var result = await connection.QueryAsync<SwabbrUser>(sql, new { VlogId = vlogId }).ConfigureAwait(false);
                if (result == null || !result.Any()) { throw new EntityNotFoundException(nameof(SwabbrUser)); }
                if (result.Count() > 1) { throw new MultipleEntitiesFoundException(nameof(SwabbrUser)); }
                return result.First();
            }
        }

        /// <summary>
        /// Retrieves our <see cref="UserSettings"/> object for a given <see cref="SwabbrUser"/>.
        /// </summary>
        /// <param name="userId">The internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="UserSettings"/></returns>
        public async Task<UserSettings> GetUserSettingsAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();
            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"SELECT *, id AS user_id FROM {ViewUserSettings} WHERE id = '{userId}';";
                var result = await connection.QueryAsync<UserSettings>(sql).ConfigureAwait(false);
                if (result == null || !result.Any()) { throw new EntityNotFoundException($"Could not find User with id = {userId}"); }
                else
                {
                    return result.First();
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="UserStatistics"/> for a given <see cref="SwabbrUser"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="UserStatistics"/></returns>
        public async Task<UserStatistics> GetUserStatisticsAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"SELECT * FROM {ViewUserStatistics} WHERE id = @Id";
                var result = await connection.QueryAsync<UserStatistics>(sql, new { Id = userId }).ConfigureAwait(false);
                if (result == null || !result.Any()) { throw new EntityNotFoundException(); }
                if (result.Count() > 1) { throw new MultipleEntitiesFoundException(); }
                return result.First();
            }
        }

        /// <summary>
        /// Gets all <see cref="SwabbrUser"/>s that we can send a vlogrequest
        /// at this very moment.
        /// </summary>
        /// <remarks>
        /// This query is very complex and hard to optimize. This function might
        /// have a long execution time.
        /// TODO Optimize
        /// </remarks>
        /// <returns><see cref="SwabbrUser"/> collection</returns>
        public async Task<IEnumerable<SwabbrUser>> GetVlogRequestableUsersAsync(DateTimeOffset from, TimeSpan timeSpan)
        {
            if (from == null) { throw new ArgumentNullException(nameof(from)); }
            if (timeSpan == null) { throw new ArgumentNullException(nameof(timeSpan)); }
            if (swabbrConfiguration.DailyVlogRequestLimit < 0) { throw new ConfigurationRangeException(nameof(swabbrConfiguration.DailyVlogRequestLimit)); }

            var to = from.AddTicks(timeSpan.Ticks);
            var sqlFrom = SqlUtility.FormatDateTime(from);
            var sqlTo = SqlUtility.FormatDateTime(to);

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    SELECT * FROM {TableUser}
                    WHERE daily_vlog_request_limit > 0
                    AND (
                        id IN (
                            SELECT u.id FROM {TableUser} AS u
                            JOIN {TableRequest} AS r
                            ON u.id = r.user_id
                            WHERE r.create_date >= '{sqlFrom}'
                            AND r.create_date <= '{sqlTo}'
                            AND u.id IN (
                                SELECT us.id
                                FROM {TableUser} AS us
                                JOIN {TableRequest} AS re
                                ON us.id = re.user_id
                                GROUP BY us.id
                                HAVING COUNT(re.id) < LEAST(us.daily_vlog_request_limit, {swabbrConfiguration.DailyVlogRequestLimit})
                            )
                        )
                        OR id NOT IN (
                            SELECT u.id FROM {TableUser} AS u
                            JOIN {TableRequest} AS r
                            ON u.id = r.user_id
                            WHERE r.create_date >= '{sqlFrom}'
                            AND r.create_date <= '{sqlTo}'
                        )
                    )
                    FOR UPDATE";
                return await connection.QueryAsync<SwabbrUser>(sql).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Updates a <see cref="SwabbrUser"/> in our database.
        /// </summary>
        /// <param name="entity"><see cref="SwabbrUser"/></param>
        /// <returns>Updated and queried <see cref="SwabbrUser"/></returns>
        public async Task<SwabbrUser> UpdateAsync(SwabbrUser entity)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
            entity.Id.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var connection = _databaseProvider.GetConnectionScope())
                {
                    await GetAsync(entity.Id).ConfigureAwait(false); // FOR UPATE

                    // TODO Enum injection
                    var sql = $@"
                        UPDATE {TableUser} SET
                            birth_date = @BirthDate,
                            country = @Country,
                            first_name = @FirstName,
                            gender = '{entity.Gender.GetEnumMemberAttribute()}', 
                            is_private = @IsPrivate,
                            last_name = @LastName,
                            nickname = @NickName,
                            profile_image_url = @ProfileImageUrl,
                            timezone = @Timezone
                        WHERE id = @Id";
                    int rowsAffected = await connection.ExecuteAsync(sql, entity).ConfigureAwait(false);
                    if (rowsAffected <= 0) { throw new EntityNotFoundException(); }
                    if (rowsAffected > 1) { throw new MultipleEntitiesFoundException(); }

                    var result = await GetAsync(entity.Id).ConfigureAwait(false);
                    scope.Complete();
                    return result;
                }
            }
        }

        /// <summary>
        /// Update the user location.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public async Task UpdateLocationAsync(Guid userId, double longitude, double latitude)
        {
            userId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    UPDATE {TableUser}
                    SET 
                        longitude = @Longitude,
                        latitude = @Latitude
                    WHERE id = @Id";
                var pars = new
                {
                    Id = userId,
                    Longitude = longitude,
                    Latitude = latitude
                };
                var rowsAffected = await connection.ExecuteAsync(sql, pars).ConfigureAwait(false);
                if (rowsAffected <= 0) { throw new EntityNotFoundException(nameof(SwabbrUser)); }
                if (rowsAffected > 1) { throw new MultipleEntitiesFoundException(nameof(SwabbrUser)); }
            }
        }

        /// <summary>
        /// Update the user timezone.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newTimeZone"></param>
        /// <returns></returns>
        public async Task UpdateTimeZoneAsync(Guid userId, TimeZoneInfo newTimeZone)
        {
            userId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    UPDATE {TableUser}
                    SET timezone = @TimeZone
                    WHERE id = @Id";
                var pars = new
                {
                    Id = userId,
                    TimeZone = newTimeZone
                };
                var rowsAffected = await connection.ExecuteAsync(sql, pars).ConfigureAwait(false);
                if (rowsAffected <= 0) { throw new EntityNotFoundException(nameof(SwabbrUser)); }
                if (rowsAffected > 1) { throw new MultipleEntitiesFoundException(nameof(SwabbrUser)); }
            }
        }

        /// <summary>
        /// Updates <see cref="UserSettings"/> in our database.
        /// </summary>
        /// <param name="entity"><see cref="UserSettings"/></param>
        /// <returns>Updated and queried <see cref="UserSettings"/></returns>
        public async Task UpdateUserSettingsAsync(UserSettings userSettings)
        {
            if (userSettings == null) { throw new ArgumentNullException(nameof(userSettings)); }
            if (userSettings.DailyVlogRequestLimit < 0) { throw new ArgumentOutOfRangeException(nameof(userSettings.DailyVlogRequestLimit)); }
            userSettings.UserId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                // TODO SQL Injection
                var sql = $@"
                    UPDATE {TableUser} SET
                        daily_vlog_request_limit = @DailyVlogRequestLimit,
                        follow_mode = '{userSettings.FollowMode.GetEnumMemberAttribute()}',
                        is_private = @IsPrivate
                    WHERE id = @UserId";
                var rowsAffected = await connection.ExecuteAsync(sql, userSettings).ConfigureAwait(false);
                if (rowsAffected < 0 ) { throw new InvalidOperationException(); }
                if (rowsAffected == 0) { throw new EntityNotFoundException(); }
                if (rowsAffected > 1) { throw new MultipleEntitiesFoundException(); }
            }
        }

        /// <summary>
        /// Checks if a <see cref="SwabbrUser"/> exists in our database.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="true"/> if it exists</returns>
        public Task<bool> UserExistsAsync(Guid userId)
        {
            return SharedRepositoryFunctions.ExistsAsync(_databaseProvider, TableUser, userId);
        }
    }

}
