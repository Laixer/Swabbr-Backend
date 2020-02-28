using Dapper;
using Laixer.Infra.Npgsql;
using Laixer.Utility.Extensions;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
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

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public UserRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
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
        /// Retrieves our <see cref="UserSettings"/> object for a given <see cref="SwabbrUser"/>.
        /// </summary>
        /// <param name="userId">The internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="UserSettings"/></returns>
        public async Task<UserSettings> GetUserSettingsAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();
            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"SELECT * FROM {ViewUserSettings} WHERE id = '{userId}';";
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
                if (result.Count() > 1) { throw new InvalidOperationException("Found multiple on single get"); }
                return result.First();
            }
        }

        public Task<IEnumerable<SwabbrUser>> SearchAsync(string query, uint offset, uint limit)
        {
            // Do we ever need this? I don't think so? --> stats
            throw new NotImplementedException();
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
                    if (rowsAffected > 1) { throw new InvalidOperationException("Found multiple results on single get"); }

                    var result = await GetAsync(entity.Id).ConfigureAwait(false);
                    scope.Complete();
                    return result;
                }
            }
        }

        public Task<UserSettings> UpdateUserSettingsAsync(UserSettings userSettings)
        {
            throw new NotImplementedException();
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
