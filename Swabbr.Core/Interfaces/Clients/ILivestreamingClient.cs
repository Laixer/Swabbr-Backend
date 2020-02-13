using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Clients
{
    public interface ILivestreamingClient
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
        /// <param name="livestreamId">Id of the livestream.</param>
        Task<LivestreamConnectionDetails> GetStreamConnectionAsync(string livestreamId);

        /// <summary>
        /// Returns connection details for an available livestream from the pool, creates a new
        /// livestream if none are available.
        /// </summary>
        /// <param name="userId">Id of the user to reserve the livestream for.</param>
        Task<LivestreamConnectionDetails> ReserveLiveStreamForUserAsync(Guid userId);

        /// <summary>
        /// Delete a new livestream.
        /// </summary>
        /// <param name="livestreamId">Id of the livestream.</param>
        Task DeleteLivestreamAsync(string livestreamId);

        /// <summary>
        /// Start a stopped livestream.
        /// </summary>
        /// <param name="livestreamId">Id of the livestream.</param>
        Task StartLivestreamAsync(string livestreamId);

        /// <summary>
        /// Stop a started livestream.
        /// </summary>
        /// <param name="livestreamId">Id of the livestream.</param>
        Task StopLivestreamAsync(string livestreamId);

        /// <summary>
        /// Fetch the playback of a livestream.
        /// </summary>
        /// <param name="livestreamId">Id of the livestream.</param>
        Task<LivestreamPlaybackDetails> GetStreamPlaybackAsync(string livestreamId);

        /// <summary>
        /// Fetches the thumbnail URL of the given stream.
        /// </summary>
        /// <param name="livestreamId">Id of the livestream.</param>
        Task<string> GetThumbnailUrlAsync(string livestreamId);

        /// <summary>
        /// Synchronize the recordings from a livestream to a vlog.
        /// </summary>
        /// <param name="livestreamId">Id of the livestream.</param>
        /// <param name="vlogId">Id of the vlog.</param>
        /// <returns></returns>
        Task GetRecordingsAsync(string livestreamId);
    }
}
