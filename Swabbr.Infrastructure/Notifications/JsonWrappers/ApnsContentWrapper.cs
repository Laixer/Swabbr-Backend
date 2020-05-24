namespace Swabbr.Core.Notifications.JsonWrappers
{

    /// <summary>
    /// Content wrapper json object template for Apple Push Notification Services.
    /// </summary>
    internal sealed class ApnsContentWrapper : NotificationJsonBase
    {

        public ApnsContentAps Aps { get; set; }

    }

    /// <summary>
    /// Content wrapper json object template for Apple Push Notification Services.
    /// </summary>
    internal sealed class ApnsContentAps
    {

        public ApnsContentAlert Alert { get; set; }

    }

    /// <summary>
    /// Content wrapper json object template for Apple Push Notification Services.
    /// </summary>
    internal sealed class ApnsContentAlert
    {

        public string Title { get; set; }

        public string Body { get; set; }

        public int ContentAvailable { get; set; }

    }

}
