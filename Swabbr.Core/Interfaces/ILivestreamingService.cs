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
        Task Create();

        /// <summary>
        /// Start a livestream.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Start(string id);

        /// <summary>
        /// Update a livestream.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Update(string id);

        /// <summary>
        /// Stop a livestream.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Stop(string id);

        /// <summary>
        /// Resets a livestream.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Reset(string id);

        /// <summary>
        /// Fetch a livestream.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Get(string id);
    }
}