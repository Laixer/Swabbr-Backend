using Swabbr.Core.Notifications.JsonWrappers;
using System;

namespace Swabbr.Api.ViewModels.Notifications.JsonWrappers
{

    /// <summary>
    /// Wrapper for our own notification data.
    /// TODO Copypasted to ignore weird <see cref="DateTimeOffset"/> display
    /// in swagger doc.
    /// </summary>
    public sealed class NotificationPayloadJsonWrapperModel
    {

        public string Protocol { get; set; }

        public string ProtocolVersion { get; set; }

        public string DataType { get; set; }

        public string DataTypeVersion { get; set; }

        public string ClickAction { get; set; }

        public string ContentType { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string UserAgent { get; set; }

        public ParametersJsonBase Data { get; set; }

    }

}
