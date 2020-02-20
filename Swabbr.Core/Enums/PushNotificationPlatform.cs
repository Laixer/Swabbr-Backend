using System.Runtime.Serialization;

namespace Swabbr.Core.Enums
{

    /// <summary>
    /// Represents the supported mobile platform services for push notifications.
    /// </summary>
    public enum PushNotificationPlatform
    {

        /// <summary>
        /// Apple Push Notification Service (iOS).
        /// </summary>
        [EnumMember(Value = "apns")]
        APNS,

        /// <summary>
        /// Firebase Cloud Messaging (Android).
        /// </summary>
        [EnumMember(Value = "fcm")]
        FCM

    }

}