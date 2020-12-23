using Swabbr.Core.Context;
using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Repository for Vlog entities.
    /// </summary>
    public interface IVlogRepository : IRepository<Vlog, Guid>,
        ICreateRepository<Vlog, Guid>,
        IDeleteRepository<Vlog, Guid>,
        IUpdateRepository<Vlog, Guid>
    {
        /// <summary>
        ///     Adds views for given vlogs.
        /// </summary>
        /// <param name="context">Context for adding vlog views.</param>
        Task AddViews(AddVlogViewsContext context);

        /// <summary>
        ///     Returns a collection of featured vlogs.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Featured vlogs.</returns>
        IAsyncEnumerable<Vlog> GetFeaturedVlogsAsync(Navigation navigation);

        /// <summary>
        ///     Gets a collection of most recent vlogs for a user.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>The most recent vlogs owned by the user.</returns>
        IAsyncEnumerable<Vlog> GetMostRecentVlogsForUserAsync(Navigation navigation);

        /// <summary>
        ///     Returns a collection of vlogs that are 
        ///     owned by the specified user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlogs that belong to the user.</returns>
        IAsyncEnumerable<Vlog> GetVlogsByUserAsync(Guid userId, Navigation navigation);
    }
}
