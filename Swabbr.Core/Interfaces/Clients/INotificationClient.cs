using Swabbr.Core.Entities;
using Swabbr.Core.Notifications;
using Swabbr.Core.Types;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Clients
{
    /// <summary>
    ///     Contract for a notification client. This should be used to
    ///     actually send notifications to whatever platform is used.
    /// </summary>
    public interface INotificationClient : ITestableService
    {
        /// <summary>
        ///     Registers a user for notifications on the external service.
        /// </summary>
        /// <param name="internalRegistration">Registration object.</param>
        /// <returns>Updated registration object.</returns>
        public Task<NotificationRegistration> RegisterAsync(NotificationRegistration internalRegistration);

        /// <summary>
        ///     Sends a notification to a user.
        /// </summary>
        /// <param name="pushDetails">Details on how to reach the user.</param>
        /// <param name="notification">The object to send.</param>
        public Task SendNotificationAsync(UserPushNotificationDetails pushDetails, SwabbrNotification notification);

        /// <summary>
        ///     Registers a user for notifications on the external service.
        /// </summary>
        /// <param name="internalRegistration">Current registration object.</param>
        public Task UnregisterAsync(NotificationRegistration internalRegistration);
    }
}
