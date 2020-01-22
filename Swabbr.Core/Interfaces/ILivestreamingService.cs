using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    public interface ILivestreamingService
    {
        /// <summary>
        /// Create a new livestream.
        /// </summary>
        /// <param name="name">Name of the livestream.</param>
        /// <param name="userId">Id of the user this stream should belong to.</param>
        /// <returns>Connection details for broadcasting the stream.</returns>
        Task<Livestream> CreateNewStreamAsync(string name);

        /// <summary>
        /// Fetches the connection details for an existing stream
        /// </summary>
        /// <returns>Connection details for broadcasting the stream.</returns>
        Task<StreamConnectionDetails> GetStreamConnectionAsync(string id);

        /// <summary>
        /// Returns connection details for an available livestream from the pool, creates a new
        /// livestream if none are available.
        /// </summary>
        /// <param name="userId">Id of the user to reserve the livestream for.</param>
        Task<StreamConnectionDetails> ReserveLiveStreamForUserAsync(Guid userId);

        /// <summary>
        /// Delete a new livestream.
        /// </summary>
        Task DeleteStreamAsync(string id);

        /// <summary>
        /// Start a stopped livestream.
        /// </summary>
        /// <param name="id">Id of the livestream.</param>
        Task StartStreamAsync(string id);

        /// <summary>
        /// Stop a started livestream.
        /// </summary>
        /// <param name="id">Id of the livestream.</param>
        Task StopStreamAsync(string id);

        /// <summary>
        /// Resets a livestream.
        /// </summary>
        /// <param name="id">Id of the livestream.</param>
        Task ResetStreamAsync(string id);

        /// <summary>
        /// Fetch the playback of a livestream.
        /// </summary>
        /// <param name="id">Id of the livestream.</param>
        Task<StreamPlaybackDetails> GetStreamPlaybackAsync(string id);

        /// <summary>
        /// Fetches the thumbnail URL of the given stream.
        /// </summary>
        /// <param name="id">Id of the livestream.</param>
        Task<Uri> GetThumbnailUrlAsync(string id);
    }
}