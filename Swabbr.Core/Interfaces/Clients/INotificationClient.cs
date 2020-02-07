using Swabbr.Core.Entities;
using Swabbr.Core.Notifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Clients
{
    public interface INotificationClient
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
        /// <param name="tags">Notification tags</param>
        Task<NotificationRegistration> RegisterAsync(DeviceRegistration deviceRegistration, IList<string> tags);

        /// <summary>
        /// Send out a notification
        /// </summary>
        /// <typeparam name="T">The notification outcome type</typeparam>
        /// <param name="notification">The notification to send</param>
        /// <param name="tags">Notification tags</param>
        Task<NotificationResponse> SendNotification(SwabbrNotification notification, PushNotificationPlatform platform, IList<string> tags);
    }
}
