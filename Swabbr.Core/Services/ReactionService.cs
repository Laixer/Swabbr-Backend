using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    public class ReactionService : IReactionService
    {
        private readonly IReactionRepository _reactionRepository;

        public ReactionService(
                IReactionRepository reactionRepository
            )
        {
            _reactionRepository = reactionRepository;
        }

        public Task<bool> ExistsAsync(Guid reactionId)
        {
            return _reactionRepository.ExistsAsync(reactionId);
        }

        public Task<Reaction> GetByIdAsync(Guid reactionId)
        {
            return _reactionRepository.GetByIdAsync(reactionId);
        }

        public Task<int> GetGivenReactionCountForUserAsync(Guid userId)
        {
            return _reactionRepository.GetGivenReactionCountForUserAsync(userId);
        }

        public Task<int> GetReactionCountForVlogAsync(Guid vlogId)
        {
            return _reactionRepository.GetReactionCountForVlogAsync(vlogId);
        }

        public Task<IEnumerable<Reaction>> GetReactionsByUserAsync(Guid userId)
        {
            return _reactionRepository.GetReactionsByUserAsync(userId);
        }

        public Task<IEnumerable<Reaction>> GetReactionsForVlogAsync(Guid vlogId)
        {
            return _reactionRepository.GetReactionsForVlogAsync(vlogId);
        }
    }
}
