namespace Swabbr.Core.Notifications
{
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