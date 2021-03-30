using Swabbr.Core.Context;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
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
    ///     Repository for vlogs.
    /// </summary>
    /// <remarks>
    ///     This repository often points to the vlog_up_to_date table. 
    ///     This is a view which only contains the vlog entities which
    ///     have their status set to up_to_date. This prevents us from
    ///     having to filter for this status each time.
    /// </remarks>
    internal class VlogRepository : DatabaseContextBase, IVlogRepository
    {
        // FUTURE: Check if the current user is allowed to watch the requested vlog.
        /// <summary>
        ///     Adds views for given vlogs.
        /// </summary>
        /// <param name="viewsContext">Context for adding vlog views.</param>
        public async Task AddViews(AddVlogViewsContext viewsContext)
        {
            if (viewsContext is null)
            {
                throw new ArgumentNullException(nameof(viewsContext));
            }

            var sql = "";
            foreach (var (vlogId, views) in viewsContext.VlogViewPairs)
            {
                sql = ExtendSql(sql, vlogId, views);
            }

            await using var context = await CreateNewDatabaseContext(sql);

            await context.NonQueryAsync();

            // Append a new sql statement for a vlog id and its views
            // to an already existing sql string.
            static string ExtendSql(string sql, Guid vlogId, uint views)
            {
                // TODO This is SQL injection
                var newSql = @$"
                    UPDATE  entities.vlog_up_to_date AS v
                    SET     views = views + {views}
                    WHERE   v.id = '{vlogId}';";

                return $"{sql}\n{newSql}";
            }
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
                        length,
                        user_id
                    )
                    VALUES (
                        @id,
                        @is_private,
                        @length,
                        @user_id
                    )";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", entity.Id);

            MapToWriter(context, entity);

            await context.NonQueryAsync();

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
                        FROM    entities.vlog_up_to_date AS v
                        WHERE   v.id = @id
                    )";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", id);

            return await context.ScalarAsync<bool>();
        }

        /// <summary>
        ///     Gets all vlogs from our data store.
        /// </summary>
        /// <remarks>
        ///     This can order by <see cref="Vlog.DateCreated"/>.
        /// </remarks>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlog result set.</returns>
        public async IAsyncEnumerable<Vlog> GetAllAsync(Navigation navigation)
        {
            var sql = @"
                SELECT  v.date_created,
                        v.id,
                        v.is_private,
                        v.length,
                        v.user_id,
                        v.views,
                        v.vlog_status
                FROM    entities.vlog_up_to_date AS v";

            sql = ConstructNavigation(sql, navigation, "v.date_created");

            await using var context = await CreateNewDatabaseContext(sql);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets all vlog wrappers from our data store.
        /// </summary>
        /// <remarks>
        ///     This can order by <see cref="Vlog.DateCreated"/>.
        /// </remarks>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlog result set.</returns>
        public async IAsyncEnumerable<VlogWrapper> GetAllWrappersAsync(Navigation navigation)
        {
            var sql = @"
                 SELECT	-- Vlog
		                    vw.vlog_date_created,
		                    vw.vlog_id,
		                    vw.vlog_is_private,
		                    vw.vlog_length,
		                    vw.vlog_user_id,
		                    vw.vlog_views,
		                    vw.vlog_vlog_status,
		
		                    -- User
		                    vw.user_birth_date,
		                    vw.user_country,
		                    vw.user_daily_vlog_request_limit,
		                    vw.user_first_name,
		                    vw.user_follow_mode,
		                    vw.user_gender,
		                    vw.user_id,
		                    vw.user_is_private,
		                    vw.user_last_name,
		                    vw.user_latitude,
		                    vw.user_longitude,
		                    vw.user_nickname,
		                    vw.user_profile_image_date_updated,
		                    vw.user_timezone,
		
		                    -- Meta data for vlog
		                    vw.vlog_like_count,
		                    vw.reaction_count
                    FROM	entities.vlog_wrapper AS vw";

            sql = ConstructNavigation(sql, navigation, "vw.vlog_date_created");

            await using var context = await CreateNewDatabaseContext(sql);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return new VlogWrapper
                {
                    Vlog = MapFromReader(reader),
                    User = UserRepository.MapFromReader(reader, 7),
                    VlogLikeCount = reader.GetInt(21),
                    ReactionCount = reader.GetInt(22)
                };
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
                            v.length,
                            v.user_id,
                            v.views,
                            v.vlog_status
                    FROM    entities.vlog_up_to_date AS v
                    WHERE   v.id = @id
                    LIMIT   1";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", id);

            await using var reader = await context.ReaderAsync();

            return MapFromReader(reader);
        }

        /// <summary>
        ///     Gets a vlog wrapper from our database.
        /// </summary>
        /// <param name="id">The vlog id.</param>
        /// <returns>The vlog.</returns>
        public async Task<VlogWrapper> GetWrapperAsync(Guid id)
        {
            var sql = @"
                   SELECT	-- Vlog
		                    vw.vlog_date_created,
		                    vw.vlog_id,
		                    vw.vlog_is_private,
		                    vw.vlog_length,
		                    vw.vlog_user_id,
		                    vw.vlog_views,
		                    vw.vlog_vlog_status,
		
		                    -- User
		                    vw.user_birth_date,
		                    vw.user_country,
		                    vw.user_daily_vlog_request_limit,
		                    vw.user_first_name,
		                    vw.user_follow_mode,
		                    vw.user_gender,
		                    vw.user_id,
		                    vw.user_is_private,
		                    vw.user_last_name,
		                    vw.user_latitude,
		                    vw.user_longitude,
		                    vw.user_nickname,
		                    vw.user_profile_image_date_updated,
		                    vw.user_timezone,
		
		                    -- Meta data for vlog
		                    vw.vlog_like_count,
		                    vw.reaction_count
                    FROM	entities.vlog_wrapper AS vw
                    WHERE   vw.vlog_id = @id
                    LIMIT   1";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", id);

            await using var reader = await context.ReaderAsync();

            return new VlogWrapper
            {
                Vlog = MapFromReader(reader),
                User = UserRepository.MapFromReader(reader, 7),
                VlogLikeCount = reader.GetInt(21),
                ReactionCount = reader.GetInt(22)
            };
        }

        // FUTURE: Implement
        /// <summary>
        ///     Returns a collection of featured vlogs.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The user id is extracted from the context.    
        ///     </para>
        ///     <para>
        ///         This currently just returns all vlogs.
        ///     </para>
        /// </remarks>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Featured vlogs.</returns>
        public IAsyncEnumerable<Vlog> GetFeaturedVlogsAsync(Navigation navigation)
            => GetAllAsync(navigation);

        // FUTURE: Implement
        /// <summary>
        ///     Returns a collection of featured vlog wrappers.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The user id is extracted from the context.    
        ///     </para>
        ///     <para>
        ///         This currently just returns all vlogs.
        ///     </para>
        /// </remarks>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Featured vlogs.</returns>
        public IAsyncEnumerable<VlogWrapper> GetFeaturedVlogWrappersAsync(Navigation navigation)
            => GetAllWrappersAsync(navigation);

        /// <summary>
        ///     Gets a collection of most recent vlogs for a user
        ///     based on all users the user follows.
        /// </summary>
        /// <remarks>
        ///     This ignores <see cref="Navigation.SortingOrder"/> as
        ///     it will always order by <see cref="Vlog.DateCreated"/>
        ///     in descending manner.
        /// </remarks>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>The most recent vlogs owned by the user.</returns>
        public async IAsyncEnumerable<Vlog> GetMostRecentVlogsForUserAsync(Navigation navigation)
        {
            if (!AppContext.HasUser)
            {
                throw new NotAllowedException();
            }

            var sql = @"
                SELECT      v.date_created,
                            v.id,
                            v.is_private,
                            v.length,
                            v.user_id,
                            v.views,
                            v.vlog_status
                FROM        entities.vlog_up_to_date AS v
                JOIN        application.follow_request_accepted AS fra
                ON          fra.requester_id = @user_id
                            AND 
                            fra.receiver_id = v.user_id
                ORDER BY    v.date_created DESC";

            sql = ConstructNavigation(sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", AppContext.UserId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets a collection of most recent vlog wrappers for a user
        ///     based on all users the user follows.
        /// </summary>
        /// <remarks>
        ///     This ignores <see cref="Navigation.SortingOrder"/> as
        ///     it will always order by <see cref="Vlog.DateCreated"/>
        ///     in descending manner.
        /// </remarks>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>The most recent vlogs owned by the user.</returns>
        public async IAsyncEnumerable<VlogWrapper> GetMostRecentVlogWrappersForUserAsync(Navigation navigation)
        {
            if (!AppContext.HasUser)
            {
                throw new NotAllowedException();
            }

            var sql = @"
                SELECT	    -- Vlog
		                    vw.vlog_date_created,
		                    vw.vlog_id,
		                    vw.vlog_is_private,
		                    vw.vlog_length,
		                    vw.vlog_user_id,
		                    vw.vlog_views,
		                    vw.vlog_vlog_status,
		
		                    -- User
		                    vw.user_birth_date,
		                    vw.user_country,
		                    vw.user_daily_vlog_request_limit,
		                    vw.user_first_name,
		                    vw.user_follow_mode,
		                    vw.user_gender,
		                    vw.user_id,
		                    vw.user_is_private,
		                    vw.user_last_name,
		                    vw.user_latitude,
		                    vw.user_longitude,
		                    vw.user_nickname,
		                    vw.user_profile_image_date_updated,
		                    vw.user_timezone,
		
		                    -- Meta data for vlog
		                    vw.vlog_like_count,
		                    vw.reaction_count
                FROM	    entities.vlog_wrapper AS vw
                JOIN        application.follow_request_accepted AS fra
                ON          fra.requester_id = @user_id
                            AND 
                            fra.receiver_id = vw.user_id
                ORDER BY    vw.vlog_date_created DESC";

            sql = ConstructNavigation(sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", AppContext.UserId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return new VlogWrapper
                {
                    Vlog = MapFromReader(reader),
                    User = UserRepository.MapFromReader(reader, 7),
                    VlogLikeCount = reader.GetInt(21),
                    ReactionCount = reader.GetInt(22)
                };
            }
        }
      
        /// <summary>
        ///     Returns a collection of vlogs that are 
        ///     owned by the specified user.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The user id is extracted from the context.
        ///     </para>
        ///     <para>
        ///         This can order by <see cref="Vlog.DateCreated"/>.
        ///     </para>
        /// </remarks>
        /// <param name="userId">Owner user id.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlogs that belong to the user.</returns>
        public async IAsyncEnumerable<Vlog> GetVlogsByUserAsync(Guid userId, Navigation navigation)
        {
            var sql = @"
                SELECT      v.date_created,
                            v.id,
                            v.is_private,
                            v.length,
                            v.user_id,
                            v.views,
                            v.vlog_status
                FROM        entities.vlog_up_to_date AS v
                WHERE       v.user_id = @user_id";

            sql = ConstructNavigation(sql, navigation, "v.date_created");

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", userId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Returns a collection of vlog wrappers that are 
        ///     owned by the specified user.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The user id is extracted from the context.
        ///     </para>
        ///     <para>
        ///         This can order by <see cref="Vlog.DateCreated"/>.
        ///     </para>
        /// </remarks>
        /// <param name="userId">Owner user id.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlogs that belong to the user.</returns>
        public async IAsyncEnumerable<VlogWrapper> GetVlogWrappersByUserAsync(Guid userId, Navigation navigation)
        {
            var sql = @"
                SELECT	    -- Vlog
		                    vw.vlog_date_created,
		                    vw.vlog_id,
		                    vw.vlog_is_private,
		                    vw.vlog_length,
		                    vw.vlog_user_id,
		                    vw.vlog_views,
		                    vw.vlog_vlog_status,
		
		                    -- User
		                    vw.user_birth_date,
		                    vw.user_country,
		                    vw.user_daily_vlog_request_limit,
		                    vw.user_first_name,
		                    vw.user_follow_mode,
		                    vw.user_gender,
		                    vw.user_id,
		                    vw.user_is_private,
		                    vw.user_last_name,
		                    vw.user_latitude,
		                    vw.user_longitude,
		                    vw.user_nickname,
		                    vw.user_profile_image_date_updated,
		                    vw.user_timezone,
		
		                    -- Meta data for vlog
		                    vw.vlog_like_count,
		                    vw.reaction_count
                FROM	    entities.vlog_wrapper AS vw
                WHERE       vw.user_id = @user_id";

            sql = ConstructNavigation(sql, navigation, "vw.vlog_date_created");

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", userId);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return new VlogWrapper
                {
                    Vlog = MapFromReader(reader),
                    User = UserRepository.MapFromReader(reader, 7),
                    VlogLikeCount = reader.GetInt(21),
                    ReactionCount = reader.GetInt(22)
                };
            }
        }

        /// <summary>
        ///     Updates a vlog in our database.
        /// </summary>
        /// <remarks>
        ///     This expects the current user to own the vlog.
        /// </remarks>
        /// <param name="entity">The vlog with updated properties.</param>
        public async Task UpdateAsync(Vlog entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (!AppContext.HasUser || !AppContext.IsUser(entity.UserId))
            {
                throw new NotAllowedException();
            }

            var sql = @"
                    UPDATE  entities.vlog_up_to_date AS v
                    SET     is_private = @is_private,
                            length = @length
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
                Length = reader.GetSafeUInt(3 + offset),
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
            context.AddParameterWithValue("length", entity.Length);
            context.AddParameterWithValue("user_id", entity.UserId);
        }
    }
}
