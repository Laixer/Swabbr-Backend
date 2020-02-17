using Laixer.Infra.Npgsql;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="VlogLike"/> entities.
    /// </summary>
    public sealed class VlogLikeRepository : IVlogLikeRepository
    {

        private IDatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogLikeRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
        }

        public Task<VlogLike> CreateAsync(VlogLike entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(VlogLike entity)
        {
            throw new NotImplementedException();
        }

        public Task<VlogLike> GetAsync(Guid id)
        {
            throw new NotImplementedException();
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
