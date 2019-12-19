using Swabbr.Core.Notifications;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    // TODO Fix comments
    public interface INotificationService
    {
        /// <summary>
        /// Create a new registration Id
        /// </summary>
        /// <returns></returns>
        Task<string> CreateRegistrationIdAsync();

        /// <summary>
        /// Delete the registration for an already registered device
        /// </summary>
        /// <param name="registrationId">Id of the registered device</param>
        Task DeleteRegistrationAsync(string registrationId);

        /// <summary>
        /// Enable push notifications for a device
        /// </summary>
        /// <param name="id">Id of the device</param>
        /// <param name="deviceUpdate">Registration information</param>
        Task<NotificationResponse> RegisterForPushNotificationsAsync(string id, NotificationDeviceRegistration deviceUpdate);

        /// <summary>
        /// Send out a notification
        /// </summary>
        /// <typeparam name="T">The notification outcome type</typeparam>
        /// <param name="notification">The notification to send</param>
        /// <returns>A response containing</returns>
        Task<NotificationResponse> SendNotificationAsync(SwabbrNotification notification);
    }
}