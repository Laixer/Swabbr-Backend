using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using Swabbr.Infrastructure.Abstractions;
using Swabbr.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{
    /// <summary>
    ///     Repository for notification registrations.
    /// </summary>
    /// <remarks>
    ///     For notification registations in the database
    ///     the id property is used as the user id.
    /// </remarks>
    internal class NotificationRegistrationRepository : RepositoryBase, INotificationRegistrationRepository
    {
        /// <summary>
        ///     Creates a new notification registration.
        /// </summary>
        /// <remarks>
        ///     The entity.Id property of <paramref name="entity"/>
        ///     should be the registering users id.
        /// </remarks>
        /// <param name="entity">The populated entity.</param>
        /// <returns>The created id.</returns>
        public async Task<Guid> CreateAsync(NotificationRegistration entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var sql = @"
                    INSERT INTO application.notification_registration(
                        external_id,
                        handle,
                        id,
                        push_notification_platform)
                    VALUES (
                        @external_id,
                        @handle,
                        @id,
                        @push_notification_platform)
                    RETURNING id";

            await using var context = await CreateNewDatabaseContext(sql);

            MapToWriter(context, entity);

            await using var reader = await context.ReaderAsync();

            return reader.GetGuid(0);
        }

        /// <summary>
        ///     Delete a notification registration from
        ///     the database.
        /// </summary>
        /// <param name="id">The user id.</param>
        public async Task DeleteAsync(Guid id)
        {
            var sql = @"
                    DELETE  
                    FROM    application.notification_registration AS nr
                    WHERE   nr.id = @id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", id);

            await context.NonQueryAsync();
        }

        // TODO Same as UserHasRegistrationAsync
        public Task<bool> ExistsAsync(Guid id)
            => UserHasRegistrationAsync(id);

        // TODO Remove?
        public IAsyncEnumerable<NotificationRegistration> GetAllAsync(Navigation navigation) => throw new NotImplementedException();

        /// <summary>
        ///     Gets a notification registration from our database.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <returns>The notification registration.</returns>
        public async Task<NotificationRegistration> GetAsync(Guid id)
        {
            var sql = @"
                    SELECT  nr.external_id,
                            nr.handle,
                            nr.id,
                            nr.push_notification_platform
                    FROM    application.notification_registration AS nr
                    WHERE   nr.id = @id
                    LIMIT   1";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", id);

            await using var reader = await context.ReaderAsync();

            return MapFromReader(reader);
        }

        // TODO Remove?
        public Task UpdateAsync(NotificationRegistration entity) => throw new NotImplementedException();

        /// <summary>
        ///     Checks if a user has an existing 
        ///     notification registration.
        /// </summary>
        /// <param name="userId">The user id.</param>
        public async Task<bool> UserHasRegistrationAsync(Guid userId)
        {
            var sql = @"
                    SELECT  EXISTS (
                    SELECT  1
                    FROM    application.notification_registration AS nr
                    WHERE   nr.id = @id)";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", userId);

            return await context.ScalarAsync<bool>();
        }

        /// <summary>
        ///     Maps a reader to a notification registration.
        /// </summary>
        /// <param name="reader">The reader to map from.</param>
        /// <returns>The mapped registration.</returns>
        private static NotificationRegistration MapFromReader(DbDataReader reader)
            => new NotificationRegistration
            {
                ExternalId = reader.GetString(0),
                Handle = reader.GetString(1),
                Id = reader.GetGuid(2),
                PushNotificationPlatform = reader.GetFieldValue<PushNotificationPlatform>(3)
            };

        /// <summary>
        ///     Maps a registration to a context.
        /// </summary>
        /// <param name="context">The context to map to.</param>
        /// <param name="entity">The entity to map from.</param>
        private static void MapToWriter(DatabaseContext context, NotificationRegistration entity)
        {
            context.AddParameterWithValue("external_id", entity.ExternalId);
            context.AddParameterWithValue("handle", entity.Handle);
            context.AddParameterWithValue("id", entity.Id);
            context.AddParameterWithValue("push_notification_platform", entity.PushNotificationPlatform);
        }
    }
}
