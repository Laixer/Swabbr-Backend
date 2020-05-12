using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contains functionality for managing a <see cref="Livestream"/> resource
    /// pool.
    /// </summary>
    public interface ILivestreamPoolService
    {

        Task<Livestream> TryGetLivestreamFromPoolAsync();

        Task<Livestream> CreateLivestreamAsync();

        Task CleanupLivestreamAsync(Guid livestreamId);

        Task CleanupTimedOutLivestreamAsync(Guid livestreamId);

        Task CleanupNeverConnectedLivestreamAsync(Guid livestreamId);

    }

}
