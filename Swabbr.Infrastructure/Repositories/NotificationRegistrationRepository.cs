using Dapper;
using Swabbr.Core.Entities;
using Swabbr.Core.Extensions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using Swabbr.Infrastructure.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Swabbr.Infrastructure.Database.DatabaseConstants;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="NotificationRegistration"/> entities.
    /// </summary>
    public sealed class NotificationRegistrationRepository : INotificationRegistrationRepository
    {

        private readonly IDatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public NotificationRegistrationRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
        }

        /// <summary>
        /// Creates a new <see cref="NotificationRegistration"/> in our database.
        /// </summary>
        /// <param name="entity"><see cref="NotificationRegistration"/></param>
        /// <returns>Created and queried <see cref="NotificationRegistration"/></returns>
        public async Task<NotificationRegistration> CreateAsync(NotificationRegistration entity)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }

            // TODO Enum injection
            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    INSERT INTO {TableNotificationRegistration} (
                        external_id,
                        handle,
                        push_notification_platform,
                        user_id
                    ) VALUES (
                        @ExternalId,
                        @Handle,
                        '{entity.PushNotificationPlatform.GetEnumMemberAttribute()}',
                        @UserId
                    ) RETURNING id";
            var id = await connection.ExecuteScalarAsync<Guid>(sql, entity).ConfigureAwait(false);
            id.ThrowIfNullOrEmpty();
            return await GetAsync(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes a <see cref="NotificationRegistration"/> from our database.
        /// </summary>
        /// <param name="id">Internal <see cref="NotificationRegistration"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task DeleteAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();
            return SharedRepositoryFunctions.DeleteAsync(_databaseProvider, id, TableNotificationRegistration);
        }

        public Task<bool> ExistsAsync(Guid id) => throw new NotImplementedException();
        public IAsyncEnumerable<NotificationRegistration> GetAllAsync(Navigation navigation) => throw new NotImplementedException();

        /// <summary>
        /// Gets a single <see cref="NotificationRegistration"/> from our database.
        /// </summary>
        /// <param name="id">Internal <see cref="NotificationRegistration"/> id</param>
        /// <returns><see cref="NotificationRegistration"/></returns>
        public Task<NotificationRegistration> GetAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();
            return SharedRepositoryFunctions.GetAsync<NotificationRegistration>(_databaseProvider, id, TableNotificationRegistration);
        }

        /// <summary>
        /// Gets all <see cref="NotificationRegistration"/>s that belong to a
        /// specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="NotificationRegistration"/> collection</returns>
        public async Task<IEnumerable<NotificationRegistration>> GetForUserAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $"SELECT * FROM {TableNotificationRegistration} WHERE user_id = @UserId";
            return await connection.QueryAsync<NotificationRegistration>(sql, new { UserId = userId }).ConfigureAwait(false);
        }

        public Task<NotificationRegistration> UpdateAsync(NotificationRegistration entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserHasRegistrationAsync(Guid userId) => throw new NotImplementedException();
        Task<Guid> IRepository<NotificationRegistration, Guid>.CreateAsync(NotificationRegistration entity) => throw new NotImplementedException();
        Task IRepository<NotificationRegistration, Guid>.UpdateAsync(NotificationRegistration entity) => throw new NotImplementedException();
    }
}
