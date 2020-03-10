using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Contract for a <see cref="NotificationRegistration"/> repository.
    /// </summary>
    public interface INotificationRegistrationRepository : IRepository<NotificationRegistration, Guid>, ICudFunctionality<NotificationRegistration, Guid>
    {

        Task<IEnumerable<NotificationRegistration>> GetRegistrationsForUserAsync(Guid userId);

    }

}
