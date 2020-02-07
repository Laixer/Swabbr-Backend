using Swabbr.Core.Notifications;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    /// A service for notification related actions.
    /// </summary>
    public interface INotificationService
    {
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
        Task<NotificationResponse> RegisterUserForPushNotificationsAsync(Guid userId, DeviceRegistration deviceUpdate);

        /// <summary>
        /// Send out a notification
        /// </summary>
        /// <typeparam name="T">The notification outcome type</typeparam>
        /// <param name="notification">The notification to send</param>
        /// <returns>A response containing</returns>
        Task<NotificationResponse> SendNotificationToUserAsync(SwabbrNotification notification, Guid userId);
    }
}