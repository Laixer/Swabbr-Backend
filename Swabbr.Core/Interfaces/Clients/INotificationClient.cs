using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Notifications;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Clients
{
    /// <summary>
    /// Contract for communicating with some external notification provider. This
    /// item has no knowledge of our internal data store.
    /// </summary>
    public interface INotificationClient
    {
        Task<bool> IsServiceAvailableAsync();

        Task<NotificationRegistration> RegisterAsync(NotificationRegistration notificationRegistration);

        Task UnregisterAsync(NotificationRegistration notificationRegistration);

        Task SendNotificationAsync(Guid userId, PushNotificationPlatform platform, SwabbrNotification notification);
    }
}
