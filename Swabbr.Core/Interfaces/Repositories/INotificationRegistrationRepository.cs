using Swabbr.Core.Entities;
using System;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Contract for a notification registration repository.
    /// </summary>
    public interface INotificationRegistrationRepository : IRepository<NotificationRegistration, Guid>,
        ICreateRepository<NotificationRegistration, Guid>,
        IDeleteRepository<NotificationRegistration, Guid>
    {
    }
}
