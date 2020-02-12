namespace Swabbr.Core.Notifications
{
    public sealed class SwabbrNotification
    {
        /// <summary>
        /// The content of the notification as specified by the messaging protocol
        /// </summary>
        // TODO THOMAS Why is this wrapped like this
        public SwabbrNotificationBody MessageContent { get; set; }
    }
}