using Swabbr.Core.Entities;
using Swabbr.Core.Types;
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
using System.Transactions;

namespace Swabbr.Infrastructure.Repositories
{
    /// <summary>
    ///     Repository for reactions.
    /// </summary>
    internal class ReactionRepository : RepositoryBase, IReactionRepository
    {
        /// <summary>
        ///     Creates a new reaction in our database.
        /// </summary>
        /// <remarks>
        ///     The returned id is the id of the given
        ///     <paramref name="entity"/> since this is
        ///     used as primary key in the database.
        /// </remarks>
        /// <param name="entity">The reaction to create.</param>
        /// <returns>The created reactions id.</returns>
        public async Task<Guid> CreateAsync(Reaction entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var sql = @"
                    INSERT INTO entities.reaction (
                        id,
                        is_private,
                        length_in_seconds,
                        target_vlog_id,
                        user_id
                    )
                    VALUES (
                        @id,
                        @is_private,
                        @length_in_seconds,
                        @target_vlog_id,
                        @user_id
                    )";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", entity.Id);

            MapToWriter(context, entity);

            // Override the user id by extracting it from the context.
            context.AddParameterWithValue("user_id", AppContext.UserId);

            await context.NonQueryAsync();

            return entity.Id;
        }

        /// <summary>
        ///     Soft deletes a reaction from our data store.
        /// </summary>
        /// <remarks>
        ///     This expects the current user to own the reaction.
        ///     If not, an <see cref="NotAllowedException"/> is 
        ///     thrown.
        /// </remarks>
        /// <param name="id">The reaction id to delete.</param>
        public async Task DeleteAsync(Guid id)
        {
            var sql = @"
                    UPDATE      entities.reaction AS r
                    SET         reaction_status = 'deleted'
                    WHERE       r.id = @id
                    RETURNING   user_id";

            // Note: To validate user ownership without an extra 
            //       db roundtrip this has to be transactional.
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", id);

            var userId = await context.ScalarAsync<Guid>();
            if (userId != AppContext.UserId)
            {
                throw new NotAllowedException();
            }

            scope.Complete();
        }

        /// <summary>
        ///     Checks if a reaction with given id exists.
        /// </summary>
        /// <param name="id">The reaction id to check.</param>
        public async Task<bool> ExistsAsync(Guid id)
        {
            var sql = @"
                    SELECT  EXISTS (
                        SELECT  1
                        FROM    entities.reaction_up_to_date AS r
                        WHERE   r.id = @id
                    )";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", id);

            return await context.ScalarAsync<bool>();
        }

        /// <summary>
        ///     Gets all reactions from our database.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Reaction result set.</returns>
        public async IAsyncEnumerable<Reaction> GetAllAsync(Navigation navigation)
        {
            var sql = @"
                    SELECT  r.date_created,
                            r.id,
                            r.is_private,
                            r.length_in_seconds,
                            r.reaction_status,
                            r.target_vlog_id,
                            r.user_id
                    FROM    entities.reaction_up_to_date AS r";

            ConstructNavigation(ref sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets a reaction from our database.
        /// </summary>
        /// <param name="id">The reaction id.</param>
        /// <returns>The reaction.</returns>
        public async Task<Reaction> GetAsync(Guid id)
        {
            var sql = @"
                    SELECT  r.date_created,
                            r.id,
                            r.is_private,
                            r.length_in_seconds,
                            r.reaction_status,
                            r.target_vlog_id,
                            r.user_id
                    FROM    entities.reaction_up_to_date AS r
                    WHERE   r.id = @id
                    LIMIT   1";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", id);

            await using var reader = await context.ReaderAsync();

            return MapFromReader(reader);
        }

        /// <summary>
        ///     Get the amount of reactions for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to check.</param>
        /// <returns>The reaction count.</returns>
        public async Task<uint> GetCountForVlogAsync(Guid vlogId)
        {
            var sql = @"
                    SELECT  count(r.*)
                    FROM    entities.reaction_up_to_date AS r
                    WHERE   r.target_vlog_id = @target_vlog_id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("target_vlog_id", vlogId);

            return (uint) await context.ScalarAsync<long>();
        }

        /// <summary>
        ///     Gets reactions that belong to a vlog.
        /// </summary>
        /// <param name="vlogId">The corresponding vlog.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Reactions for the vlog.</returns>
        public async IAsyncEnumerable<Reaction> GetForVlogAsync(Guid vlogId, Navigation navigation)
        {
            var sql = @"
                    SELECT  r.date_created,
                            r.id,
                            r.is_private,
                            r.length_in_seconds,
                            r.reaction_status,
                            r.target_vlog_id,
                            r.user_id
                    FROM    entities.reaction_up_to_date AS r";

            ConstructNavigation(ref sql, navigation);

            await using var context = await CreateNewDatabaseContext(sql);

            await foreach (var reader in context.EnumerableReaderAsync())
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Updates a reaction in our data store.
        /// </summary>
        /// <remarks>
        ///     This throws a <see cref="NotAllowedException"/> 
        ///     if the reaction is not owned by the current user.
        /// </remarks>
        /// <param name="entity">The reaction with updated properties.</param>
        public async Task UpdateAsync(Reaction entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (!AppContext.HasUser || entity.UserId != AppContext.UserId)
            {
                throw new NotAllowedException();
            }

            var sql = @"
                    UPDATE  entities.reaction_up_to_date AS r
                    SET     is_private = @is_private,
                            length_in_seconds = @length_in_seconds
                    WHERE   r.id = @id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("id", entity.Id);

            MapToWriter(context, entity);

            await context.NonQueryAsync();
        }

        /// <summary>
        ///     Maps a reader to a reaction.
        /// </summary>
        /// <param name="reader">The reader to map from.</param>
        /// <param name="offset">Ordinal offset.</param>
        /// <returns>The mapped reaction.</returns>
        internal static Reaction MapFromReader(DbDataReader reader, int offset = 0)
            => new Reaction
            {
                DateCreated = reader.GetDateTime(0 + offset),
                Id = reader.GetGuid(1 + offset),
                IsPrivate = reader.GetBoolean(2 + offset),
                LengthInSeconds = reader.GetSafeUInt(3 + offset),
                ReactionStatus = reader.GetFieldValue<ReactionStatus>(4 + offset),
                TargetVlogId = reader.GetGuid(5 + offset),
                UserId = reader.GetGuid(6 + offset)
            };

        /// <summary>
        ///     Maps a reaction to a database context.
        /// </summary>
        /// <param name="context">The context to map to.</param>
        /// <param name="entity">The entity to map from.</param>
        private static void MapToWriter(DatabaseContext context, Reaction entity)
        {
            context.AddParameterWithValue("is_private", entity.IsPrivate);
            context.AddParameterWithValue("length_in_seconds", entity.LengthInSeconds);
            context.AddParameterWithValue("reaction_status", entity.ReactionStatus);
            context.AddParameterWithValue("target_vlog_id", entity.TargetVlogId);
            context.AddParameterWithValue("user_id", entity.UserId);
        }
    }
}
