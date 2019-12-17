using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    //TODO: Unfinished
    //TODO: Add options?
    public interface ILivestreamingService
    {
        /// <summary>
        /// Create a new livestream.
        /// </summary>
        /// <returns></returns>
        Task CreateStreamAsync();

        /// <summary>
        /// Start a livestream.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task StartStreamAsync(string id);

        /// <summary>
        /// Update a livestream.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task UpdateStreamAsync(string id);

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
        Task GetStreamAsync(string id);
    }
}