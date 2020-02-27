using Dapper;
using Laixer.Infra.Npgsql;
using Laixer.Utility.Extensions;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Swabbr.Infrastructure.Database.DatabaseConstants;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="Vlog"/> entities.
    /// </summary>
    public sealed class VlogRepository : IVlogRepository
    {

        private readonly IDatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
        }

        /// <summary>
        /// Creates a new <see cref="Vlog"/> in our database.
        /// </summary>
        /// <param name="entity"><see cref="Vlog"/></param>
        /// <returns><see cref="Vlog"/> with id assigned to it</returns>
        public async Task<Vlog> CreateAsync(Vlog entity)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
            entity.Id.ThrowIfNotNullOrEmpty();
            entity.UserId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                // TODO DRY
                // TODO Inserting a new vlog that is a reaction should never get a livestream id
                // This will create a possibility where the livestream_id will be {0000-00-00...}.
                var sql = $@"
                    INSERT INTO {TableVlog} (
                        is_private,
                        user_id,
                        livestream_id
                    ) VALUES (
                        @IsPrivate,
                        @UserId,
                        @LivestreamId
                    ) RETURNING id";
                var id = await connection.ExecuteScalarAsync<Guid>(sql, entity).ConfigureAwait(false);
                id.ThrowIfNullOrEmpty();
                entity.Id = id;
                return entity;
            }
        }

        /// <summary>
        /// Deletes a <see cref="Vlog"/> from our database.
        /// </summary>
        /// <remarks>
        /// This throws an <see cref="Core.Exceptions.EntityNotFoundException"/> if we can't find the entity.
        /// </remarks>
        /// <param name="id">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task DeleteAsync(Guid id) => SharedRepositoryFunctions.DeleteAsync(_databaseProvider, id, TableVlog);

        public Task<bool> ExistsAsync(Guid vlogId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if a <see cref="Vlog"/> exists for a specified <see cref="Livestream"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="true"/> if exists</returns>
        public async Task<bool> ExistsForLivestreamAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"SELECT 1 FROM {TableVlog} WHERE livestream_id = @Id FOR UPDATE"; // TODO Does this row lock?
                var pars = new { Id = livestreamId };
                var result = await connection.QueryAsync<int>(sql, pars).ConfigureAwait(false);
                if (result == null || !result.Any()) { return false; }
                if (result.Count() > 1) { throw new InvalidOperationException("Multiple entities after single get"); }
                return true;
            }
        }

        /// <summary>
        /// Gets a <see cref="Vlog"/> from our database.
        /// </summary>
        /// <param name="id">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Vlog"/>/returns>
        public Task<Vlog> GetAsync(Guid id) => SharedRepositoryFunctions.GetAsync<Vlog>(_databaseProvider, id, TableVlog);

        public Task<IEnumerable<Vlog>> GetFeaturedVlogsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Guid>> GetSharedUserIdsAsync(Guid vlogId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetVlogCountForUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a <see cref="Vlog"/> based on a <see cref="Livestream"/>, since
        /// they are 1-1 linked.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Vlog"/> linked to the <see cref="Livestream"/></returns>
        public async Task<Vlog> GetVlogFromLivestreamAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"SELECT * FROM public.vlog WHERE livestream_id = @Id";
                var pars = new { Id = livestreamId };
                var result = await connection.QueryAsync<Vlog>(sql, pars).ConfigureAwait(false);
                if (result == null || !result.Any()) { throw new EntryPointNotFoundException(); }
                if (result.Count() > 1) { throw new InvalidOperationException("Found multiple result for single get"); }
                return result.First();
            }
        }

        public Task<IEnumerable<Vlog>> GetVlogsByUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task ShareWithUserAsync(Guid vlogId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<Vlog> UpdateAsync(Vlog entity)
        {
            throw new NotImplementedException();
        }

    }

}
