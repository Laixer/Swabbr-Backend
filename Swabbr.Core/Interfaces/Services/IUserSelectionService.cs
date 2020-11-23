using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for managing hash distributions for timed triggers.
    /// </summary>
    public interface IUserSelectionService
    {
        /// <summary>
        ///     Gets all users that are selected for a 
        ///     given moment in time based on our hash.
        /// </summary>
        /// <param name="time">The moment in time to check.</param>
        /// <param name="offset">The offset, used for timezones.</param>
        /// <returns>The users that are selected by the hash.</returns>
        IAsyncEnumerable<SwabbrUser> GetForMinuteAsync(DateTimeOffset time, TimeSpan? offset = null);
    }
}
