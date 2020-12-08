namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Represents the supported mobile platform services for push notifications.
    /// </summary>
    public enum PushNotificationPlatform
    {
        /// <summary>
        /// Apple Push Notification Service (iOS).
        /// </summary>
        APNS = 0,

        /// <summary>
        /// Firebase Cloud Messaging (Android).
        /// </summary>
        FCM = 1
    }
}