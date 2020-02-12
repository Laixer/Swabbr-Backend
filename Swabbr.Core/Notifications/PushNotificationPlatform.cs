namespace Swabbr.Core.Notifications
{
    /// <summary>
    /// Represents the supported mobile platform services for push notifications
    /// </summary>
    /// TODO THOMAS Move to Swabbr.Core.Enums
    public enum PushNotificationPlatform
    {
        /// <summary>
        /// Apple Push Notification Service (iOS)
        /// </summary>
        APNS,

        /// <summary>
        /// Firebase Cloud Messaging (Android)
        /// </summary>
        FCM
    }
}