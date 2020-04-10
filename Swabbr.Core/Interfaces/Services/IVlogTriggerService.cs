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

        Task ProcessVlogTriggersAsync(DateTimeOffset time);

        Task ProcessVlogTriggerForUserAsync(Guid userId, DateTimeOffset triggerMinute);

        Task ProcessVlogTimeoutsAsync(DateTimeOffset time);

        Task ProcessVlogTimeoutForUserAsync(Guid userId, DateTimeOffset triggerMinute);

    }

}
