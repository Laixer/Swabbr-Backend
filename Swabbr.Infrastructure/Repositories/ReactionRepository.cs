using Dapper;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Extensions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Infrastructure.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Swabbr.Infrastructure.Database.DatabaseConstants;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="Reaction"/> entities.
    /// </summary>
    public sealed class ReactionRepository : IReactionRepository
    {

        private readonly IDatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public ReactionRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
        }

        /// <summary>
        /// Creates a new <see cref="Reaction"/> in our database.
        /// </summary>
        /// <param name="entity"><see cref="Reaction"/></param>
        /// <returns>The created and queried <see cref="Reaction"/></returns>
        public async Task<Reaction> CreateAsync(Reaction entity)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
            entity.Id.ThrowIfNotNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            // State is done automatically
            var sql = $@"
                    INSERT INTO {TableReaction} (
                        is_private,
                        target_vlog_id,
                        user_id
                    ) VALUES (
                        @IsPrivate,
                        @TargetVlogId,
                        @UserId
                    ) RETURNING id";
            var id = await connection.ExecuteScalarAsync<Guid>(sql, entity).ConfigureAwait(false);
            id.ThrowIfNullOrEmpty();
            return await GetAsync(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a <see cref="Reaction"/> from our database.
        /// </summary>
        /// <param name="id">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Reaction"/></returns>
        public async Task<Reaction> GetAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT *
                    FROM {TableReaction} 
                    WHERE id = @Id
                    AND reaction_state != '{ReactionState.Deleted.GetEnumMemberAttribute()}'";
            var result = await connection.QueryAsync<Reaction>(sql, new { Id = id }).ConfigureAwait(false);
            if (result == null || !result.Any()) { throw new EntityNotFoundException(nameof(Reaction)); }
            if (result.Count() > 1) { throw new MultipleEntitiesFoundException(nameof(Reaction)); }
            return result.First();
        }

        /// <summary>
        /// Gets all <see cref="Reaction"/>s for a given <see cref="Vlog"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Reaction"/> collection</returns>
        public async Task<IEnumerable<Reaction>> GetForVlogAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT * FROM {TableReaction} 
                    WHERE target_vlog_id = @VlogId 
                    AND reaction_state = '{ReactionState.UpToDate.GetEnumMemberAttribute()}'";
            return await connection.QueryAsync<Reaction>(sql, new { VlogId = vlogId }).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the amount of <see cref="Reaction"/>s for a given <paramref name="vlogId"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Reaction"/> count</returns>
        public async Task<int> GetReactionCountForVlogAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT COUNT(*) 
                    FROM {TableReaction} 
                    WHERE target_vlog_id = @VlogId
                    AND reaction_state = '{ReactionState.UpToDate.GetEnumMemberAttribute()}'";
            return await connection.ExecuteScalarAsync<int>(sql, new { VlogID = vlogId }).ConfigureAwait(false);
        }

        /// <summary>
        /// Hard deletes a <see cref="Reaction"/> from the data store. This should
        /// be used to clean up failed uploads and such.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task HardDeleteAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $"DELETE FROM {TableReaction} WHERE id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = reactionId }).ConfigureAwait(false);
            if (rowsAffected == 0) { throw new EntityNotFoundException(nameof(Reaction)); }
            if (rowsAffected > 1) { throw new MultipleEntitiesFoundException(nameof(Reaction)); }
        }

        /// <summary>
        /// Soft deletes a <see cref="Reaction"/> from our database. The reaction
        /// will be marked as <see cref="ReactionState.Deleted"/>.
        /// </summary>
        /// <param name="reactionId"></param>
        /// <returns></returns>
        public Task SoftDeleteAsync(Guid reactionId)
        {
            return MarkAsStatusAsync(reactionId, ReactionState.Deleted);
        }

        /// <summary>
        /// Updates a <see cref="Reaction"/> in our database.
        /// </summary>
        /// <remarks>
        /// This can't update the <see cref="Reaction.ReactionState"/>.
        /// TODO This should never be able to update a reaction in <see cref="ReactionState.Deleted"/>.
        /// </remarks>
        /// <param name="entity"><see cref="Reaction"/></param>
        /// <returns>Updated and queried <see cref="Reaction"/></returns>
        public async Task<Reaction> UpdateAsync(Reaction entity)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
            entity.Id.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    UPDATE {TableReaction} SET 
                        is_private = @IsPrivate
                    WHERE id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, entity).ConfigureAwait(false);
            if (rowsAffected == 0) { throw new EntityNotFoundException(); }
            if (rowsAffected > 1) { throw new InvalidOperationException("Found multiple items on single get"); }

            return await GetAsync(entity.Id).ConfigureAwait(false);
        }

        /// <summary>
        /// Marks a <see cref="Reaction"/> as <paramref name="state"/>.
        /// </summary>
        /// <remarks>
        /// This will throw if the database determines an invalid state change.
        /// </remarks>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <param name="state"><see cref="ReactionState"/></param>
        /// <returns><see cref="Task"/></returns>
        private async Task MarkAsStatusAsync(Guid reactionId, ReactionState state)
        {
            reactionId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            // TODO SQL injection
            var sql = $@"
                    UPDATE {TableReaction} 
                    SET reaction_state = '{state.GetEnumMemberAttribute()}'
                    WHERE id = @ReactionId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { ReactionId = reactionId }).ConfigureAwait(false);
            if (rowsAffected <= 0) { throw new EntityNotFoundException(nameof(Reaction)); }
            if (rowsAffected > 1) { throw new MultipleEntitiesFoundException(nameof(Reaction)); }
        }

    }

}
