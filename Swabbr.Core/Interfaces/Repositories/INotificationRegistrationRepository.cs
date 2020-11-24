using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Contract for a notification registration repository.
    /// </summary>
    public interface INotificationRegistrationRepository : IRepository<NotificationRegistration, Guid>
    {
        /// <summary>
        ///     Checks if a user has an existing 
        ///     notification registration.
        /// </summary>
        /// <param name="userId">The user id.</param>
        Task<bool> UserHasRegistrationAsync(Guid userId);
    }
}
