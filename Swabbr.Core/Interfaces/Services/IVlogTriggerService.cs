using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for handling vlog trigger requests.
    /// </summary>
    public interface IVlogTriggerService
    {

        /// <summary>
        /// Called when a user is selected by our algorithm to start vlogging.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        Task ProcessVlogTriggerForUserAsync(Guid userId);

        Task ProcessVlogTriggerTimoutAsync(Guid userId);

    }

}
