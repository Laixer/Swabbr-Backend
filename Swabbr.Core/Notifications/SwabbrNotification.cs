﻿using Swabbr.Core.Notifications.JsonWrappers;
using System;

namespace Swabbr.Core.Notifications
{
    /// <summary>
    ///     Represents a single notification to be sent to a user.
    /// </summary>
    /// <remarks>
    ///     This contains a lot of metadata. The actual content
    ///     of the notification exists in <see cref="Data"/>.
    /// </remarks>
    public sealed class SwabbrNotification
    {
        /// <summary>
        ///     Constructor to force us to always initialize 
        ///     <see cref="NotificationAction"/>.
        /// </summary>
        /// <param name="notificationAction">The type of notification.</param>
        /// <param name="data"><see cref="ParametersJsonBase"/>.</param>
        /// <param name="title">The title to display.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="protocol">Protocol type.</param>
        /// <param name="protocolVersion">Protocol version.</param>
        public SwabbrNotification(NotificationAction notificationAction,
            ParametersJsonBase data,
            string title = null,
            string message = null,
            string protocol = "swabbr",
            string protocolVersion = "1.0")
        {
            ClickAction = NotificationActionTranslator.Translate(notificationAction);
            ClickActionInt = notificationAction;

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

        /// <summary>
        ///     The notification protocol used.
        /// </summary>
        public string Protocol { get; }

        /// <summary>
        ///     The version of the protocol used.
        /// </summary>
        public string ProtocolVersion { get; }

        /// <summary>
        ///     The type of data sent.
        /// </summary>
        public string DataType { get; }

        /// <summary>
        ///     The version of the data sent.
        /// </summary>
        public string DataTypeVersion { get; }

        /// <summary>
        ///     The type of notification, <see cref="NotificationAction"/>,
        ///     as a string value.
        /// </summary>
        public string ClickAction { get; }

        /// <summary>
        ///     The type of notification, <see cref="NotificationAction"/>.
        /// </summary>
        public NotificationAction ClickActionInt { get; }

        /// <summary>
        ///     The content type.
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        ///     The moment the notification was sent.
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        ///     The user agent which sent the notification.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        ///     Contains the actual notification data.
        /// </summary>
        public ParametersJsonBase Data { get; set; }
    }
}
