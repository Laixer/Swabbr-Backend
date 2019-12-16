namespace Swabbr.Api.Services.NotificationHubs
{
    /// <summary>
    /// Represents the supported mobile platform services for push notifications
    /// </summary>
    public enum MobilePlatform
    {
        /// <summary>
        /// Apple Push Notification Service (iOS)
        /// </summary>
        APNS,

        /// <summary>
        /// FireBase Cloud Messaging (Android)
        /// </summary>
        FCM
    }
}
