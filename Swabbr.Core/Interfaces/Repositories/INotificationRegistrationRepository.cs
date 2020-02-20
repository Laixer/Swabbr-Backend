using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Contract for a <see cref="NotificationRegistration"/> repository.
    /// </summary>
    public interface INotificationRegistrationRepository : IRepository<NotificationRegistration, Guid>, ICudFunctionality<NotificationRegistration, Guid>
    {

        /// <summary>
        /// Get a notification registration record by providing the user and registration id.
        /// </summary>
        Task<NotificationRegistration> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Checks if a registration exists for a specific user.
        /// </summary>
        Task<bool> ExistsForUser(Guid userId);

    }

}
