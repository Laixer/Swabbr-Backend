using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for handling vlog trigger requests.
    /// </summary>
    public interface IVlogTriggerService
    {

        Task ProcessVlogTriggerForUserAsync(Guid userId, DateTimeOffset triggerMinute);

        Task ProcessVlogTimeoutForUserAsync(Guid userId, DateTimeOffset triggerMinute);

    }

}
