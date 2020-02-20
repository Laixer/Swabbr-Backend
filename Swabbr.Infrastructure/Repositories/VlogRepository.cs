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
    /// Repository for <see cref="Vlog"/> entities.
    /// </summary>
    public sealed class VlogRepository : IVlogRepository
    {

        private IDatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
        }

        public Task<Vlog> CreateAsync(Vlog entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Guid vlogId)
        {
            throw new NotImplementedException();
        }

        public Task<Vlog> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Vlog> GetByIdAsync(Guid vlogId)
        {
            throw new NotImplementedException();
        }

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
