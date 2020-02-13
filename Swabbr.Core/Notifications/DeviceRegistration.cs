using Swabbr.Core.Enums;

namespace Swabbr.Core.Notifications
{

    /// <summary>
    /// Represents a device registration.
    /// TODO Shouldn't this just be an entity?
    /// </summary>
    public class DeviceRegistration
    {

        /// <summary>
        /// PNS platform of the device
        /// </summary>
        public PushNotificationPlatform Platform { get; set; }

        /// <summary>
        /// PNS handle of the device
        /// </summary>
        public string Handle { get; set; }

    }

}
