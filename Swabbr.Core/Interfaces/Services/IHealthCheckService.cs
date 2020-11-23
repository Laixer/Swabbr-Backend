using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for checking the health of our backend.
    /// </summary>
    public interface IHealthCheckService
    {
        /// <summary>
        ///     Checks if our backend is healthy.
        /// </summary>
        /// <remarks>
        ///     This can generally be used as a wrapper
        ///     call to call all other checks in this 
        ///     interface.
        /// </remarks>
        Task<bool> IsHealthyAsync();

        /// <summary>
        ///     Checks our data store.
        /// </summary>
        Task<bool> IsDataStoreHealthyAsync();

        /// <summary>
        ///     Checks our notification service.
        /// </summary>
        Task<bool> IsNotificationServiceHealthyAsync();
    }
}
