namespace Swabbr.Infrastructure.Notifications.JsonWrappers
{
    // FUTURE Implement.
    /// <summary>
    ///     Content wrapper json object template for 
    ///     Apple Push Notification Services.
    /// </summary>
    internal sealed class ApnsContentWrapper : NotificationWrapperJsonBase
    {
        /// <summary>
        ///     Subwrapper.
        /// </summary>
        public ApnsContentAps Aps { get; set; }
    }

    /// <summary>
    ///     Content wrapper json object template 
    ///     for Apple Push Notification Services.
    /// </summary>
    internal sealed class ApnsContentAps
    {
        /// <summary>
        ///     APNS specific subwrapper.
        /// </summary>
        public ApnsContentAlert Alert { get; set; }
    }

    /// <summary>
    ///     Content wrapper json object template for 
    ///     Apple Push Notification Services.
    /// </summary>
    internal sealed class ApnsContentAlert
    {
        /// <summary>
        ///     Notification title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Notification body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///     Content available indicator.
        /// </summary>
        public int ContentAvailable { get; set; }
    }
}
