namespace Swabbr.Core.Notifications
{
    public sealed class SwabbrNotification
    {
        /// <summary>
        /// PNS platform of the device to send this notification to.
        /// </summary>
        public PushNotificationPlatform Platform { get; set; }

        /// <summary>
        /// The content of the notification as specified by the messaging protocol
        /// </summary>
        public SwabbrMessage MessageContent { get; set; }
    }
}