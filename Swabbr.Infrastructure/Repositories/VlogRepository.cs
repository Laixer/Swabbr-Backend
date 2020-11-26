using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using Swabbr.Infrastructure.Abstractions;
using Swabbr.Infrastructure.Database;
using Swabbr.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{
    /// <summary>
    ///     Repository for vlogs.
    /// </summary>
    internal class VlogRepository : RepositoryBase, IVlogRepository
    {
        /// <summary>
        ///     Adds a view to a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog that has been watched.</param>
        public async Task AddView(Guid vlogId)
        {
            var sql = @"
                    UPDATE  entities.vlog AS v
                    SET     views = views + 1
                    WHERE   v.id = @id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", vlogId);

            await context.NonQueryAsync();
        }

        /// <summary>
        ///     Creates a new vlog in our database.
        /// </summary>
        /// <remarks>
        ///     The returned id is the id of the given
        ///     <paramref name="entity"/> since this is
        ///     used as primary key in the database.
        /// </remarks>
        /// <param name="entity">The vlog to create.</param>
        /// <returns>The created vlogs id.</returns>
        public async Task<Guid> CreateAsync(Vlog entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var sql = @"
                    INSERT INTO entities.vlog (
                        id,
                        is_private,
                        length_in_seconds,
                        user_id
                    )
                    VALUES (
                        @id,
                        @is_private,
                        @length_in_seconds,
                        @user_id
                    )";

            await using var context = await CreateNewDatabaseContext(sql);

            // Explicitly add the id.
            context.AddParameterWithValue("id", entity.Id);

            MapToWriter(context, entity);

            await using var reader = await context.ReaderAsync();

            return entity.Id;
        }

        /// <summary>
        ///     Soft deletes a vlog in our database.
        /// </summary>
        /// <param name="id">The vlog id to delete.</param>
        public async Task DeleteAsync(Guid id)
        {
            var sql = @"
                    UPDATE  entities.vlog AS v
                    SET     vlog_status = 'deleted'
                    WHERE   v.id = @id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", id);

            await context.NonQueryAsync();
        }

        /// <summary>
        ///     Checks if a vlog with given id exists.
        /// </summary>
        /// <param name="id">The id to check for.</param>
        public async Task<bool> ExistsAsync(Guid id)
        {
            var sql = @"
                    SELECT  EXISTS (
                        SELECT  1
                        FROM    entities.vlog AS v
                        WHERE   v.id = @id
                    )";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", id);

            return await context.ScalarAsync<bool>();
        }

        /// <summary>
        ///     Gets all vlogs from our data store.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlog result set.</returns>
        public async IAsyncEnumerable<Vlog> GetAllAsync(Navigation navigation)
        {
            var sql = @"
                SELECT  v.date_created,
                        v.id,
                        v.is_private,
                        v.length_in_seconds,
                        v.user_id,
                        v.views,
                        v.vlog_status
                FROM    entities.vlog AS v";

            ConstructNavigation(ref sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets a vlog from our database.
        /// </summary>
        /// <param name="id">The vlog id.</param>
        /// <returns>The vlog.</returns>
        public async Task<Vlog> GetAsync(Guid id)
        {
            var sql = @"
                    SELECT  v.date_created,
                            v.id,
                            v.is_private,
                            v.length_in_seconds,
                            v.user_id,
                            v.views,
                            v.vlog_status
                    FROM    entities.vlog AS v
                    WHERE   v.id = @id
                    LIMIT   1";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", id);

            await using var reader = await context.ReaderAsync();

            return MapFromReader(reader);
        }

        // FUTURE Implement
        /// <summary>
        ///     Returns a collection of featured vlogs.
        /// </summary>
        /// <remarks>
        ///     This currently just returns all vlogs.
        /// </remarks>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Featured vlogs.</returns>
        public IAsyncEnumerable<Vlog> GetFeaturedVlogsAsync(Navigation navigation)
            => GetAllAsync(navigation);

        /// <summary>
        ///     Gets a collection of most recent vlogs for a user
        ///     based on all users the user follows.
        /// </summary>
        /// <param name="userId">The owner of the vlogs.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>The most recent vlogs owned by the user.</returns>
        public async IAsyncEnumerable<Vlog> GetMostRecentVlogsForUserAsync(Guid userId, Navigation navigation)
        {
            var sql = @"
                SELECT      v.date_created,
                            v.id,
                            v.is_private,
                            v.length_in_seconds,
                            v.user_id,
                            v.views,
                            v.vlog_status
                FROM        entities.vlog AS v
                JOIN        application.follow_request_accepted AS fra
                ON          fra.requester_id = @user_id
                WHERE       fra.requester_id = @user_id
                ORDER BY    v.date_created DESC";

            ConstructNavigation(ref sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", userId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Returns a collection of vlogs that are 
        ///     owned by the specified user.
        /// </summary>
        /// <remarks>
        ///     This also sorts by most recent.
        /// </remarks>
        /// <param name="userId">Owner user id.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlogs that belong to the user.</returns>
        public async IAsyncEnumerable<Vlog> GetVlogsFromUserAsync(Guid userId, Navigation navigation)
        {
            var sql = @"
                SELECT      v.date_created,
                            v.id,
                            v.is_private,
                            v.length_in_seconds,
                            v.user_id,
                            v.views,
                            v.vlog_status
                FROM        entities.vlog AS v
                WHERE       v.user_id = @user_id
                ORDER BY    v.date_created DESC";

            ConstructNavigation(ref sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", userId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Updates a vlog in our database.
        /// </summary>
        /// <param name="entity">The vlog with updated properties.</param>
        public async Task UpdateAsync(Vlog entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var sql = @"
                    UPDATE  entities.vlog AS v
                    SET     is_private = @is_private,
                            length_in_seconds = @length_in_seconds
                    WHERE   v.id = @id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", entity.Id);

            MapToWriter(context, entity);

            await context.NonQueryAsync();
        }

        /// <summary>
        ///     Maps a reader to a vlog.
        /// </summary>
        /// <param name="reader">The reader to vlog from.</param>
        /// <param name="offset">Ordinal offset.</param>
        /// <returns>The mapped vlog.</returns>
        internal static Vlog MapFromReader(DbDataReader reader, int offset = 0)
            => new Vlog
            {
                DateCreated = reader.GetDateTime(0 + offset),
                Id = reader.GetGuid(1 + offset),
                IsPrivate = reader.GetBoolean(2 + offset),
                LengthInSeconds = reader.GetUInt(3 + offset),
                UserId = reader.GetGuid(4 + offset),
                Views = reader.GetUInt(5 + offset),
                VlogStatus = reader.GetFieldValue<VlogStatus>(6 + offset)
            };

        /// <summary>
        ///     Maps a vlog to a database context.
        /// </summary>
        /// <param name="context">The context to map to.</param>
        /// <param name="entity">The entity to map from.</param>
        internal static void MapToWriter(DatabaseContext context, Vlog entity)
        {
            context.AddParameterWithValue("is_private", entity.IsPrivate);
            context.AddParameterWithValue("length_in_seconds", entity.LengthInSeconds);
            context.AddParameterWithValue("user_id", entity.UserId);
        }
    }
}
