namespace Swabbr.Core.Notifications.Data
{
    /// <summary>
    ///     Abstract base class for creating 
    ///     operation specific JSON wrappers.
    /// </summary>
    public abstract class NotificationData
    {
        /// <summary>
        ///     The title of the notification.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     The message of the notification.
        /// </summary>
        public string Message { get; set; }
    }
}
