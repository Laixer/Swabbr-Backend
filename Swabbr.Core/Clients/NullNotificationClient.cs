using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Notifications;
using Swabbr.Core.Types;
using System.Threading.Tasks;

namespace Swabbr.Core.Clients
{
    /// <summary>
    ///     Notification client which does nothing.
    /// </summary>
    /// <remarks>
    ///     This never validates any parameters.
    /// </remarks>
    public class NullNotificationClient : INotificationClient
    {
        /// <summary>
        ///     Returns a <c>null</c> object.
        /// </summary>
        public Task<NotificationRegistration> RegisterAsync(NotificationRegistration internalRegistration) => Task.FromResult<NotificationRegistration>(null);

        /// <summary>
        ///     Does nothing and returns <see cref="Task.CompletedTask"/>;
        /// </summary>
        public Task SendNotificationAsync(UserPushNotificationDetails pushDetails, SwabbrNotification notification) => Task.CompletedTask;

        /// <summary>
        ///     Does nothing and returns <see cref="Task.CompletedTask"/>;
        /// </summary>
        public Task TestServiceAsync() => Task.CompletedTask;

        /// <summary>
        ///     Does nothing and returns <see cref="Task.CompletedTask"/>;
        /// </summary>
        public Task UnregisterAsync(NotificationRegistration internalRegistration) => Task.CompletedTask;
    }
}
