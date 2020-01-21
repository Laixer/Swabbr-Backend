namespace Swabbr.Core.Notifications
{
    public sealed class SwabbrNotification
    {
        /// <summary>
        /// The content of the notification as specified by the messaging protocol
        /// </summary>
        public SwabbrMessage MessageContent { get; set; }
    }
}