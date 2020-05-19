using Swabbr.Core.Notifications.JsonWrappers;
using System;

namespace Swabbr.Core.Notifications
{

    /// <summary>
    /// Represents a single notification to be sent to a user.
    /// </summary>
    public sealed class SwabbrNotification
    {

        /// <summary>
        /// Constructor to force us to always initialize <see cref="NotificationAction"/>.
        /// </summary>
        /// <param name="notificationAction"><see cref="NotificationAction"/></param>
        public SwabbrNotification(NotificationAction notificationAction,
            ParametersJsonBase data,
            string title = null,
            string message = null,
            string protocol = "swabbr",
            string protocolVersion = "1.0")
        {
            ClickAction = NotificationActionTranslator.Translate(notificationAction);
            Timestamp = DateTimeOffset.Now;
            Protocol = protocol;
            ProtocolVersion = protocolVersion;
            DataType = "notification";
            DataTypeVersion = "1.0";
            ContentType = "application/json";

            Data = data ?? throw new ArgumentNullException(nameof(data));
            Data.Title = title;
            Data.Message = message;

            UserAgent = "swabbr-backend"; // TODO What to do with this?
        }

        public string Protocol { get; }

        public string ProtocolVersion { get; }

        public string DataType { get; }

        public string DataTypeVersion { get; }

        public string ClickAction { get; }

        public string ContentType { get; }

        public DateTimeOffset Timestamp { get; }

        public string UserAgent { get; set; }

        public ParametersJsonBase Data { get; set; }

    }

}
