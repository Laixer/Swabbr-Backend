using Laixer.Infra.Npgsql;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="Reaction"/> entities.
    /// </summary>
    public sealed class ReactionRepository : IReactionRepository
    {

        private IDatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public ReactionRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
        }

        public Task<bool> ExistsAsync(Guid reactionId)
        {
            throw new NotImplementedException();
        }

        public Task<Reaction> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Reaction> GetByIdAsync(Guid reactionId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetGivenReactionCountForUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetReactionCountForVlogAsync(Guid vlogId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Reaction>> GetReactionsByUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Reaction>> GetReactionsForVlogAsync(Guid vlogId)
        {
            throw new NotImplementedException();
        }
    }
}
