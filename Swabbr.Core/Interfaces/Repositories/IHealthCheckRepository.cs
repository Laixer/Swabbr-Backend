using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Contract for checking the status of our data store.
    /// </summary>
    public interface IHealthCheckRepository
    {
        /// <summary>
        ///     Checks if our data store is online.
        /// </summary>
        Task<bool> IsAliveAsync();
    }
}
