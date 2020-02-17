using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    public interface INotificationRegistrationRepository : IRepository<NotificationRegistration>
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