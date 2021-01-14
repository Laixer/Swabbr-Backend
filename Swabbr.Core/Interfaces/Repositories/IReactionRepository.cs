using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Contract for a reaction repository.
    /// </summary>
    public interface IReactionRepository : IRepository<Reaction, Guid>,
        ICreateRepository<Reaction, Guid>,
        IDeleteRepository<Reaction, Guid>,
        IUpdateRepository<Reaction, Guid>
    {
        /// <summary>
        ///     Get the amount of reactions for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to check.</param>
        /// <returns>The reaction count.</returns>
        Task<uint> GetCountForVlogAsync(Guid vlogId);

        /// <summary>
        ///     Gets reactions that belong to a vlog.
        /// </summary>
        /// <param name="vlogId">The corresponding vlog.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Reactions for the vlog.</returns>
        IAsyncEnumerable<Reaction> GetForVlogAsync(Guid vlogId, Navigation navigation);
    }
}
