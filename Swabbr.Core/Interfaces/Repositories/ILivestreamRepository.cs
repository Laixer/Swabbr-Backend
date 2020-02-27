using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Repository for <see cref="Livestream"/> entities.
    /// </summary>
    public interface ILivestreamRepository : IRepository<Livestream, Guid>, ICudFunctionality<Livestream, Guid>
    {

        /// <summary>
        /// Gets the external id of a <see cref="Livestream"/>.
        /// </summary>
        /// <param name="id">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Livestream.ExternalId"/> string value</returns>
        Task<string> GetExternalIdAsync(Guid id);

        /// <summary>
        /// Sets the <see cref="Livestream.LivestreamStatus"/> property in our database.
        /// </summary>
        /// <param name="id">Internal <see cref="Livestream"/> id</param>
        /// <param name="status">New <see cref="LivestreamStatus"/></param>
        /// <returns><see cref="Task"/></returns>
        Task UpdateLivestreamStatusAsync(Guid id, LivestreamStatus status);

    }

}
