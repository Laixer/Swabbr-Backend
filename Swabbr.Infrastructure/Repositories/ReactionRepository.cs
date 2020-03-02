using Dapper;
using Laixer.Infra.Npgsql;
using Laixer.Utility.Extensions;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
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

            using (var connection = _databaseProvider.GetConnectionScope())
            {
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
        }

        /// <summary>
        /// Deletes a <see cref="Reaction"/> from our database.
        /// </summary>
        /// <param name="id">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task DeleteAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();
            return SharedRepositoryFunctions.DeleteAsync(_databaseProvider, id, TableReaction);
        }

        /// <summary>
        /// Gets a <see cref="Reaction"/> from our database.
        /// </summary>
        /// <param name="id">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Reaction"/></returns>
        public Task<Reaction> GetAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();
            return SharedRepositoryFunctions.GetAsync<Reaction>(_databaseProvider, id, TableReaction);
        }

        /// <summary>
        /// Gets all <see cref="Reaction"/>s for a given <see cref="Vlog"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Reaction"/> collection</returns>
        public async Task<IEnumerable<Reaction>> GetForVlogAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"SELECT * FROM {TableReaction} WHERE target_vlog_id = @VlogId FOR UPDATE";
                return await connection.QueryAsync<Reaction>(sql, new { VlogId = vlogId }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Updates a <see cref="Reaction"/> in our database.
        /// </summary>
        /// <param name="entity"><see cref="Reaction"/></param>
        /// <returns>Updated and queried <see cref="Reaction"/></returns>
        public async Task<Reaction> UpdateAsync(Reaction entity)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
            entity.Id.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    UPDATE {TableReaction} SET 
                        is_private = @IsPrivate
                    WHERE id = @Id";
                var rowsAffected = await connection.ExecuteAsync(sql, entity).ConfigureAwait(false);
                if (rowsAffected == 0) { throw new EntityNotFoundException(); }
                if (rowsAffected > 1) { throw new InvalidOperationException("Found multiple items on single get"); }

                return await GetAsync(entity.Id).ConfigureAwait(false);
            }
        }
    }
}
