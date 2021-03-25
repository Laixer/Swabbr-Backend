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
        ///     Gets a vlog wrapper from our data store.
        /// </summary>
        /// <param name="id">The vlog id.</param>
        /// <returns>The vlog.</returns>
        Task<VlogWrapper> GetWrapperAsync(Guid id);

        /// <summary>
        ///     Gets all vlog wrappers from our data store.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlog result set.</returns>
        IAsyncEnumerable<VlogWrapper> GetAllWrappersAsync(Navigation navigation);

        /// <summary>
        ///     Returns a collection of featured vlogs.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Featured vlogs.</returns>
        IAsyncEnumerable<Vlog> GetFeaturedVlogsAsync(Navigation navigation);

        /// <summary>
        ///     Returns a collection of featured vlog wrappers.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Featured vlogs.</returns>
        IAsyncEnumerable<VlogWrapper> GetFeaturedVlogWrappersAsync(Navigation navigation);

        /// <summary>
        ///     Gets a collection of most recent vlogs for a user.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>The most recent vlogs owned by the user.</returns>
        IAsyncEnumerable<Vlog> GetMostRecentVlogsForUserAsync(Navigation navigation);

        /// <summary>
        ///     Gets a collection of most recent vlog wrappers for a user
        ///     based on all users the user follows.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>The most recent vlogs owned by the user.</returns>
        IAsyncEnumerable<VlogWrapper> GetMostRecentVlogWrappersForUserAsync(Navigation navigation);

        /// <summary>
        ///     Returns a collection of vlogs that are 
        ///     owned by the specified user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlogs that belong to the user.</returns>
        IAsyncEnumerable<Vlog> GetVlogsByUserAsync(Guid userId, Navigation navigation);

        /// <summary>
        ///     Returns a collection of vlog wrappers that are 
        ///     owned by the specified user.
        /// </summary>
        /// <param name="userId">Owner user id.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlogs that belong to the user.</returns>
        IAsyncEnumerable<VlogWrapper> GetVlogWrappersByUserAsync(Guid userId, Navigation navigation);
    }
}
