using Swabbr.Api.ViewModels.Enums;

namespace Swabbr.Api.ViewModels.DeviceRegistration
{

    /// <summary>
    /// Viewmodel for parts of <see cref="Core.Entities.NotificationRegistration"/>.
    /// </summary>
    public sealed class DeviceRegistrationInputModel
    {

        /// <summary>
        /// PNS handle of the device
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// Indicates which platform is being used for sending push notifications.
        /// </summary>
        public PushNotificationPlatformModel Platform { get; set; }

    }

}
