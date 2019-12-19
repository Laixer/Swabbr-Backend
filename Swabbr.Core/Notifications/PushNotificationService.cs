namespace Swabbr.Core.Notifications
{
    /// <summary>
    /// Represents the supported mobile platform services for push notifications
    /// </summary>
    public enum PushNotificationService
    {
        /// <summary>
        /// Apple Push Notification Service (iOS)
        /// </summary>
        APNS,

        /// <summary>
        /// Firebase Cloud Messaging
        /// </summary>
        FCM
    }
}
