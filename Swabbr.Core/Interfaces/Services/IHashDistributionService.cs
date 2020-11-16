using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for managing hash distributions for timed triggers.
    /// </summary>
    public interface IHashDistributionService
   {
        /// <summary>
        ///     Gets all users that are selected for a 
        ///     given moment in time based on our hash.
        /// </summary>
        /// <param name="users">All eligible users.</param>
        /// <param name="time">The moment in time to check.</param>
        /// <param name="offset">The offset, used for timezones.</param>
        /// <returns>The users that are selected by the hash.</returns>
        IEnumerable<SwabbrUser> GetForMinute(IEnumerable<SwabbrUser> users, DateTimeOffset time, TimeSpan? offset = null);
    }
}
