using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.BackgroundWorkers
{

    /// <summary>
    /// Contract for managing <see cref="Entities.Livestream"/> timeouts after
    /// user requests.
    /// </summary>
    public interface ILivestreamTimeoutWorker
    {

        Task StartAsync(Guid livestreamId, TimeSpan delay);

        Task CleanupAsync(Guid livestreamId);

    }

}
