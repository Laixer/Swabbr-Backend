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
                        push_notification_platform
                    )
                    VALUES (
                        @external_id,
                        @handle,
                        @id,
                        @push_notification_platform
                    )
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

        /// <summary>
        ///     Checks if a notification registration with a
        ///     given id exists, which actually checks if a 
        ///     user has an existing notification registration.
        /// </summary>
        /// <param name="id">The user id.</param>
        public async Task<bool> ExistsAsync(Guid id)
        {
            var sql = @"
                    SELECT  EXISTS (
                        SELECT  1
                        FROM    application.notification_registration AS nr
                        WHERE   nr.id = @user_id
                    )";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", id);

            return await context.ScalarAsync<bool>();
        }

        /// <summary>
        ///     Gets all notification registrations from our database.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Notification registration result set.</returns>
        public async IAsyncEnumerable<NotificationRegistration> GetAllAsync(Navigation navigation)
        {
            var sql = @"
                    SELECT  nr.external_id,
                            nr.handle,
                            nr.id,
                            nr.push_notification_platform
                    FROM    application.notification_registration AS nr";

            ConstructNavigation(ref sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

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

        /// <summary>
        ///     Update a notification registration in the data store.
        /// </summary>
        /// <remarks>
        ///     This is invalid and returns <see cref="InvalidOperationException"/>.
        /// </remarks>
        public Task UpdateAsync(NotificationRegistration entity)
            => throw new InvalidOperationException();

        /// <summary>
        ///     Maps a reader to a notification registration.
        /// </summary>
        /// <param name="reader">The reader to map from.</param>
        /// <param name="offset">Ordinal offset.</param>
        /// <returns>The mapped registration.</returns>
        private static NotificationRegistration MapFromReader(DbDataReader reader, int offset = 0)
            => new NotificationRegistration
            {
                ExternalId = reader.GetString(0 + offset),
                Handle = reader.GetString(1 + offset),
                Id = reader.GetGuid(2 + offset),
                PushNotificationPlatform = reader.GetFieldValue<PushNotificationPlatform>(3 + offset)
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
