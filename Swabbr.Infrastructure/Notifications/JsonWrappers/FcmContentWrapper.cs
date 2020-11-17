using Swabbr.Core.Notifications;

namespace Swabbr.Infrastructure.Notifications.JsonWrappers
{
    /// <summary>
    ///     JSON wrapper template for Firebase Cloud Messaging.
    /// </summary>
    internal sealed class FcmContentWrapper : NotificationWrapperJsonBase
    {
        /// <summary>
        ///     Subwrapper.
        /// </summary>
        public SubData Data { get; set; }
    }

    /// <summary>
    ///     Subwrapper to match expected Firebase format.
    /// </summary>
    internal sealed class SubData
    {
        /// <summary>
        ///     Contains the actual notification.
        /// </summary>
        public SwabbrNotification Payload { get; set; }
    }
}
