using Swabbr.Core.Notifications.JsonWrappers;
using System;

namespace Swabbr.Api.ViewModels.Notifications.JsonWrappers
{

    /// <summary>
    /// Wrapper for our own notification data.
    /// This was copypasted to ignore weird <see cref="DateTimeOffset"/> display
    /// in swagger doc.
    /// </summary>
    public sealed class NotificationPayloadJsonWrapperModel
    {

        /// <summary>
        /// Name of the protocol.
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// Version of the protocol.
        /// </summary>
        public string ProtocolVersion { get; set; }

        /// <summary>
        /// Datatype of this message.
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Datatype version of this message.
        /// </summary>
        public string DataTypeVersion { get; set; }

        /// <summary>
        /// The <see cref="Core.Notifications.NotificationAction"/>.
        /// </summary>
        public string ClickAction { get; set; }

        /// <summary>
        /// Content encoding type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// When the message was sent.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// The user agent.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Contains our data of type <see cref="DataType"/>.
        /// </summary>
        public ParametersJsonBase Data { get; set; }

    }

}
