using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    public interface IReactionRepository : IRepository<Reaction>
    {
        /// <summary>
        /// Returns whether a reaction with the given id exists.
        /// </summary>
        /// <param name="reactionId">Unique identifier of a reaction</param>
        Task<bool> ExistsAsync(Guid reactionId);

        /// <summary>
        /// Returns a reaction with the specified id.
        /// </summary>
        /// <param name="reactionId">Unique identifier of a reaction</param>
        /// <returns></returns>
        Task<Reaction> GetByIdAsync(Guid reactionId);

        /// <summary>
        /// Returns a collection of reactions for a given vlog.
        /// </summary>
        /// <param name="vlogId">Unique identifier of the vlog the reactions were placed on</param>
        Task<IEnumerable<Reaction>> GetReactionsForVlogAsync(Guid vlogId);

        /// <summary>
        /// Returns a collection of reactions that were placed by a specific user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user who placed the reactions</param>
        Task<IEnumerable<Reaction>> GetReactionsByUserAsync(Guid userId);

        /// <summary>
        /// Returns the amount of reactions that were placed on a specific vlog.
        /// </summary>
        /// <param name="userId">Unique identifier of a vlog</param>
        Task<int> GetReactionCountForVlogAsync(Guid vlogId);

        /// <summary>
        /// Returns the amount of reactions that were placed by a specific user.
        /// </summary>
        /// <param name="userId">Unique identifier of a user</param>
        Task<int> GetGivenReactionCountForUserAsync(Guid userId);
    }
}
