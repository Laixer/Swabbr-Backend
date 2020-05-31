using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    /// Contract for checking the health of our backend.
    /// </summary>
    public interface IHealthCheckService
    {

        /// <summary>
        /// Checks if our backend is healthy.
        /// </summary>
        /// <returns><see cref="bool"/> result</returns>
        Task<bool> IsHealthyAsync();

    }
}
