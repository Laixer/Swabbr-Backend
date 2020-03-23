using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for a service that handles everything related to <see cref="Reaction"/> entities.
    /// </summary>
    public interface IReactionService
    {

        Task<Reaction> GetReactionAsync(Guid reactionId);

        // TODO This needs to incorperate the actual file sending as well!
        Task<Reaction> PostReactionAsync(Guid userId, Guid targetVlogId, bool isPrivate);

        Task<Reaction> UpdateReactionAsync(Guid userId, Guid reactionId, bool isPrivate);

        Task<IEnumerable<Reaction>> GetReactionsForVlogAsync(Guid vlogId);

        Task<int> GetReactionCountForVlogAsync(Guid vlogId);

        Task DeleteReactionAsync(Guid userId, Guid reactionId);

    }

}
