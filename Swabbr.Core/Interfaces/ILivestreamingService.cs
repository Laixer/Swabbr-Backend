using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Swabbr.Core.Entities;

namespace Swabbr.Core.Interfaces
{
    //TODO: Unfinished
    //TODO: Add options?
    public interface ILivestreamingService
    {
        /// <summary>
        /// Create a new livestream.
        /// </summary>
        /// <returns>JSON object representing the created livestream</returns>
        Task<StreamConnectionDetails> CreateNewStreamAsync(string name);

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
        /// Fetch a livestream.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<StreamConnectionDetails> GetStreamAsync(string id);
    }
}