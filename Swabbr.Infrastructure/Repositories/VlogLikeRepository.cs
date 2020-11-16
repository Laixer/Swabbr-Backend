using Dapper;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Extensions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using Swabbr.Infrastructure.Providers;
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
        public VlogLikeRepository(IDatabaseProvider databaseProvider) => _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));

        /// <summary>
        /// Creates a new <see cref="VlogLike"/> in our database.
        /// </summary>
        /// <param name="entity"><see cref="VlogLike"/></param>
        /// <returns>Created and queried <see cref="VlogLike"/></returns>
        public async Task<VlogLike> CreateAsync(VlogLike entity)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
            entity.Id.VlogId.ThrowIfNullOrEmpty();
            entity.Id.UserId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
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

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    DELETE FROM {TableVlogLike}
                    WHERE vlog_id = @VlogId
                    AND user_id = @UserId";
            var rowsAffected = await connection.ExecuteAsync(sql, vlogLikeId).ConfigureAwait(false);
            if (rowsAffected == 0) { throw new EntityNotFoundException(); }
            if (rowsAffected > 1) { throw new InvalidOperationException("Found multiple entities on single get"); }
        }

        /// <summary>
        /// Checks if a <see cref="VlogLike"/> relation between a 
        /// <see cref="SwabbrUser"/> and a <see cref="Vlog"/> exists or not.
        /// </summary>
        /// <param name="vlogLikeId">Internal <see cref="VlogLike"/> id</param>
        /// <returns><see cref="bool"/> if exists</returns>
        public async Task<bool> ExistsAsync(VlogLikeId vlogLikeId)
        {
            if (vlogLikeId == null) { throw new ArgumentNullException(nameof(vlogLikeId)); }
            vlogLikeId.VlogId.ThrowIfNullOrEmpty();
            vlogLikeId.UserId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT * FROM {TableVlogLike}
                    WHERE vlog_id = @VlogId
                    AND user_id = @UserId";
            var rowsAffected = await connection.QueryAsync(sql, vlogLikeId).ConfigureAwait(false);
            if (rowsAffected == null) { throw new InvalidOperationException("Something went wrong during exists query"); }
            if (rowsAffected.Count() > 1) { throw new InvalidOperationException("Found multiple entities on single get"); }
            return rowsAffected.Count() == 1;
        }

        /// <summary>
        /// Gets a single <see cref="VlogLike"/> from our database.
        /// </summary>
        /// <param name="vlogLikeId">Internal <see cref="VlogLike"/> id</param>
        /// <returns><see cref="VlogLike"/></returns>
        public async Task<VlogLike> GetAsync(VlogLikeId vlogLikeId)
        {
            if (vlogLikeId == null) { throw new ArgumentNullException(nameof(vlogLikeId)); }
            vlogLikeId.VlogId.ThrowIfNullOrEmpty();
            vlogLikeId.UserId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $@"
                    SELECT * FROM {TableVlogLike}  
                    WHERE vlog_id = @VlogId
                    AND user_id = @UserId";
            var result = await connection.QueryAsync<VlogLike>(sql, vlogLikeId).ConfigureAwait(false);
            if (result == null || !result.Any()) { throw new EntityNotFoundException(); }
            if (result.Count() > 1) { throw new MultipleEntitiesFoundException(); }
            return result.First();
        }

        // TODO This is not used.
        /// <summary>
        /// Gets the amount of <see cref="VlogLike"/>s given to a certain
        /// <see cref="Vlog"/>.
        /// </summary>
        /// <remarks>
        /// This does NOT check for <see cref="Vlog"/> existence.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns>Count</returns>
        public async Task<int> GetCountForVlogAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $"SELECT COUNT(*) FROM {TableVlogLike} WHERE vlog_id = @VlogId";
            return await connection.ExecuteScalarAsync<int>(sql, new { VlogId = vlogId }).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all <see cref="VlogLike"/> entities that belong to a given
        /// <see cref="Vlog"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="VlogLike"/> collection</returns>
        public async Task<IEnumerable<VlogLike>> GetAllForVlogAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();

            using var connection = _databaseProvider.GetConnectionScope();
            var sql = $"SELECT * FROM {TableVlogLike} WHERE vlog_id = @VlogId";
            return await connection.QueryAsync<VlogLike>(sql, new { VlogId = vlogId }).ConfigureAwait(false);
        }

        public Task<VlogLike> UpdateAsync(VlogLike entity) => throw new NotImplementedException();

        // TODO This should be cleaned up.
        /// <summary>
        ///     This is used by <see cref="GetVlogLikeDetailsForVlogAsync(Guid)"/>.
        /// </summary>
        private class VlogLikeMetadata
        {
            public uint Count { get; set; }

            public Guid VlogId { get; set; }

            public Guid UserId { get; set; }

            public string UserNickname { get; set; }
        }

        /// <summary>
        ///     Gets a <see cref="VlogLikeSummary"/> for a <see cref="Vlog"/>.
        /// </summary>
        /// <remarks>
        ///     The <see cref="VlogLikeSummary.SimplifiedUsers"/> field only
        ///     contains the first 5 users that liked the <see cref="Vlog"/>.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="VlogLikeSummary"/></returns>
        public async Task<VlogLikeSummary> GetVlogLikeSummaryForVlogAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();

            // TODO Table public.user should be in quotes.
            var sql = @"
                WITH cnt AS (
	                SELECT count(vlog_id) AS c FROM  vlog_like AS vl
	                WHERE vl.vlog_id = @VlogId
                )
                SELECT 
	                cnt.c AS count,
	                vlog_likes.vlog_id,
	                u.id AS user_id,
	                u.nickname AS user_nickname
                FROM cnt
                CROSS JOIN (
	                SELECT * FROM vlog_like
	                WHERE vlog_id = @VlogId
	                LIMIT 5
                ) AS vlog_likes
                JOIN public.user AS u ON vlog_likes.user_id = u.id";

            using var connection = _databaseProvider.GetConnectionScope();
            var metadata = await connection.QueryAsync<VlogLikeMetadata>(sql, new { VlogId = vlogId }).ConfigureAwait(false);

            if (!metadata.Any())
            {
                return new VlogLikeSummary
                {
                    SimplifiedUsers = new List<SwabbrUserSimplified>(),
                    TotalLikes = 0,
                    VlogId = vlogId
                };
            }
            else
            {
                return new VlogLikeSummary
                {
                    TotalLikes = metadata.First().Count,
                    VlogId = metadata.First().VlogId,
                    SimplifiedUsers = metadata.Select(x => new SwabbrUserSimplified
                    {
                        Id = x.UserId,
                        Nickname = x.UserNickname
                    })
                };
            }
        }

    }
}
