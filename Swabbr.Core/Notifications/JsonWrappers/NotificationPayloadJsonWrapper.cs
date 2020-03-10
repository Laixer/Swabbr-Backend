using System;

namespace Swabbr.Core.Notifications.JsonWrappers
{

    /// <summary>
    /// Wrapper for our own notification data.
    /// </summary>
    public sealed class NotificationPayloadJsonWrapper
    {

        public string Protocol => "swabbr";

        public string ProtocolVersion => "1.0";

        public string DataType { get; set; }

        public string DataTypeVersion { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string NotificationAction { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

    }

}
