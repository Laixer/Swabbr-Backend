using Microsoft.Azure.NotificationHubs;
using Swabbr.Core.Notifications;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    // TODO Comments
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
        /// Register a device for push notifications
        /// </summary>
        /// <param name="id">Id of the device</param>
        /// <param name="deviceUpdate">Registration information</param>
        Task<HubResponse> RegisterForPushNotificationsAsync(string id, DeviceRegistration deviceUpdate);

        /// <summary>
        /// Send out a push notification
        /// </summary>
        /// <param name="notification"></param>
        Task<HubResponse<NotificationOutcome>> SendNotificationAsync(PushNotification notification);
    }
}