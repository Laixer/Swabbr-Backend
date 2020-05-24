using System.Runtime.Serialization;

namespace Swabbr.Api.ViewModels.Enums
{

    /// <summary>
    /// Represents a push notification platform.
    /// </summary>
    public enum PushNotificationPlatformModel
    {

        /// <summary>
        /// Apple Push Notification Service (iOS)
        /// </summary>
        [EnumMember(Value = "apns")]
        APNS,

        /// <summary>
        /// Firebase Cloud Messaging (Android)
        /// </summary>
        [EnumMember(Value = "fcm")]
        FCM

    }

}
