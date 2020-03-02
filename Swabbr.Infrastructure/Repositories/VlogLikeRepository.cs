using Dapper;
using Laixer.Infra.Npgsql;
using Laixer.Utility.Extensions;
using Npgsql;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Swabbr.Infrastructure.Database.DatabaseConstants;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="VlogLike"/> entities.
    /// </summary>
    public sealed class VlogLikeRepository : IVlogLikeRepository
    {

        private readonly IDatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogLikeRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
        }

        /// <summary>
        /// Creates a new <see cref="VlogLike"/> in our database.
        /// </summary>
        /// <param name="entity"><see cref="VlogLike"/></param>
        /// <returns>Created and queried <see cref="VlogLike"/></returns>
        public async Task<VlogLike> CreateAsync(VlogLike entity)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
            entity.VlogId.ThrowIfNullOrEmpty();
            entity.UserId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    INSERT INTO {TableVlogLike} (
                        vlog_id,
                        user_id
                    ) VALUES (
                        @VlogId,
                        @UserId
                    )";
                await connection.ExecuteAsync(sql, entity).ConfigureAwait(false);

                return await GetAsync(entity.Id).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes a <see cref="VlogLike"/> from our database.
        /// </summary>
        /// <param name="vlogLikeId">Internal <see cref="VlogLike"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task DeleteAsync(VlogLikeId vlogLikeId)
        {
            if (vlogLikeId == null) { throw new ArgumentNullException(nameof(vlogLikeId)); }
            vlogLikeId.VlogId.ThrowIfNullOrEmpty();
            vlogLikeId.UserId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    DELETE FROM {TableVlogLike}
                    WHERE vlog_id = @VlogId
                    AND user_id = @UserId";
                var rowsAffected = await connection.ExecuteAsync(sql, vlogLikeId).ConfigureAwait(false);
                if (rowsAffected == 0) { throw new EntityNotFoundException(); }
                if (rowsAffected > 1) { throw new InvalidOperationException("Found multiple entities on single get"); }
            }
        }

        /// <summary>
        /// Checks if a <see cref="VlogLike"/> relation between a 
        /// <see cref="SwabbrUser"/> and a <see cref="Vlog"/> exists or not.
        /// </summary>
        /// <remarks>
        /// Contains the FOR UPDATE sql clause.
        /// </remarks>
        /// <param name="vlogLikeId">Internal <see cref="VlogLike"/> id</param>
        /// <returns><see cref="bool"/> if exists</returns>
        public async Task<bool> ExistsAsync(VlogLikeId vlogLikeId)
        {
            if (vlogLikeId == null) { throw new ArgumentNullException(nameof(vlogLikeId)); }
            vlogLikeId.VlogId.ThrowIfNullOrEmpty();
            vlogLikeId.UserId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    SELECT * FROM {TableVlogLike}
                    WHERE vlog_id = @VlogId
                    AND user_id = @UserId
                    FOR UPDATE";
                var rowsAffected = await connection.QueryAsync(sql, vlogLikeId).ConfigureAwait(false);
                if (rowsAffected == null) { throw new InvalidOperationException("Something went wrong during exists query"); }
                if (rowsAffected.Count() > 1) { throw new InvalidOperationException("Found multiple entities on single get"); }
                return rowsAffected.Count() == 1;
            }
        }

        /// <summary>
        /// Gets a single <see cref="VlogLike"/> from our database.
        /// </summary>
        /// <remarks>
        /// Contains the FOR UPDATE sql clause.
        /// </remarks>
        /// <param name="vlogLikeId">Internal <see cref="VlogLike"/> id</param>
        /// <returns><see cref="VlogLike"/></returns>
        public async Task<VlogLike> GetAsync(VlogLikeId vlogLikeId)
        {
            if (vlogLikeId == null) { throw new ArgumentNullException(nameof(vlogLikeId)); }
            vlogLikeId.VlogId.ThrowIfNullOrEmpty();
            vlogLikeId.UserId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    SELECT * FROM {TableVlogLike}  
                    WHERE vlog_id = @VlogId
                    AND user_id = @UserId
                    FOR UPDATE";
                var result = await connection.QueryAsync<VlogLike>(sql, vlogLikeId).ConfigureAwait(false);
                if (result == null || !result.Any()) { throw new EntityNotFoundException(); }
                if (result.Count() > 1) { throw new InvalidOperationException("Found more than one entity for single get"); }
                return result.First();
            }
        }

        /// <summary>
        /// Gets all <see cref="VlogLike"/> entities that belong to a given
        /// <see cref="Vlog"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="VlogLike"/> collection</returns>
        public async Task<IEnumerable<VlogLike>> GetForVlogAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"SELECT * FROM {TableVlogLike} WHERE vlog_id = @VlogId";
                return await connection.QueryAsync<VlogLike>(sql, new { VlogId = vlogId }).ConfigureAwait(false);
            }
        }

        public Task<int> GetGivenCountForUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<VlogLike> GetSingleForUserAsync(Guid vlogId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<VlogLike> UpdateAsync(VlogLike entity)
        {
            throw new NotImplementedException();
        }
    }
}
