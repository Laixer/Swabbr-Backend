using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Swabbr.Core.Entities;

namespace Swabbr.Core.Interfaces
{
    public interface ILivestreamingService
    {
        /// <summary>
        /// Create a new livestream.
        /// </summary>
        /// <returns>JSON object representing the created livestream</returns>
        Task<StreamConnectionDetails> CreateNewStreamAsync(string name);

        /// <summary>
        /// Delete a new livestream.
        /// </summary>
        Task DeleteStreamAsync(string id);

        /// <summary>
        /// Start a livestream.
        /// </summary>
        /// <param name="id">Id of the livestream.</param>
        /// <returns>JSON object representing the stream that was started</returns>
        Task StartStreamAsync(string id);

        /// <summary>
        /// Update a livestream.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>JSON object representing the updated stream.</returns>
        Task<JObject> UpdateStreamAsync(string id);

        // TODO Determine 
        /// <summary>
        /// Stop a livestream.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task StopStreamAsync(string id);

        /// <summary>
        /// Resets a livestream.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task ResetStreamAsync(string id);

        /// <summary>
        /// Fetch the connection details of a livestream.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<StreamConnectionDetails> GetStreamConnectionAsync(string id);

        /// <summary>
        /// Fetch the playback of a livestream.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<StreamPlaybackDetails> GetStreamPlaybackAsync(string id);
    }
}