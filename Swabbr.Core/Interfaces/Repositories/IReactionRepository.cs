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
        ///     Gets a reaction wrapper by id.
        /// </summary>
        /// <param name="id">The internal reaction id.</param>
        /// <returns>Reaction wrapper.</returns>
        Task<ReactionWrapper> GetWrapperAsync(Guid id);

        /// <summary>
        ///     Gets reactions that belong to a vlog.
        /// </summary>
        /// <param name="vlogId">The corresponding vlog.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Reactions for the vlog.</returns>
        IAsyncEnumerable<Reaction> GetForVlogAsync(Guid vlogId, Navigation navigation);

        /// <summary>
        ///     Gets reaction wrappers that belong to a vlog.
        /// </summary>
        /// <param name="vlogId">The corresponding vlog.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Reaction wrappers for the vlog.</returns>
        IAsyncEnumerable<ReactionWrapper> GetWrappersForVlogAsync(Guid vlogId, Navigation navigation);
    }
}
