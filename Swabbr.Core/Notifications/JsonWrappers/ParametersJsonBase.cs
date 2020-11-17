namespace Swabbr.Core.Notifications.JsonWrappers
{
    /// <summary>
    ///     Abstract base class for creating 
    ///     operation specific JSON wrappers.
    /// </summary>
    public abstract class ParametersJsonBase
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
