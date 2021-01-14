using Swabbr.Core.Notifications.Data;
using System;

namespace Swabbr.Core.Notifications
{
    /// <summary>
    ///     Represents a single notification to be sent to a user.
    /// </summary>
    /// <remarks>
    ///     This contains a lot of metadata. The actual content
    ///     of the notification data exists in <see cref="Data"/>.
    /// </remarks>
    public sealed class SwabbrNotification
    {
        /// <summary>
        ///     Constructor to force us to always initialize 
        ///     <see cref="Notifications.NotificationAction"/>.
        /// </summary>
        /// <param name="notificationAction">The type of notification.</param>
        /// <param name="data"><see cref="NotificationData"/>.</param>
        /// <param name="title">The title to display.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="protocol">Protocol type.</param>
        /// <param name="protocolVersion">Protocol version.</param>
        public SwabbrNotification(NotificationAction notificationAction,
            NotificationData data,
            string title = null,
            string message = null,
            string protocol = "swabbr",
            string protocolVersion = "1.0")
        {
            NotificationActionString = NotificationActionTranslator.Translate(notificationAction);
            NotificationAction = notificationAction;

            Timestamp = DateTimeOffset.Now;
            Protocol = protocol;
            ProtocolVersion = protocolVersion;

            Data = data ?? throw new ArgumentNullException(nameof(data));
            Data.Title = title;
            Data.Message = message;

            UserAgent = "swabbr-backend";
        }

        /// <summary>
        ///     The notification protocol used.
        /// </summary>
        public string Protocol { get; }

        /// <summary>
        ///     The version of the protocol used.
        /// </summary>
        public string ProtocolVersion { get; }

        /// <summary>
        ///     <see cref="NotificationAction"/> as a string value.
        /// </summary>
        public string NotificationActionString { get; }

        /// <summary>
        ///     The type of notification.
        /// </summary>
        public NotificationAction NotificationAction { get; }

        /// <summary>
        ///     The moment the notification was sent.
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        ///     The user agent which sent the notification.
        /// </summary>
        public string UserAgent { get; }

        /// <summary>
        ///     Contains the actual notification data.
        /// </summary>
        public NotificationData Data { get; init; }
    }
}
