namespace Swabbr.Infrastructure.Configuration
{

    /// <summary>
    /// Configuration file for Azure Notification Hub.
    /// </summary>
    public class NotificationHubConfiguration
    {

        /// <summary>
        /// Connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Name of the Hub in ANH.
        /// </summary>
        public string HubName { get; set; }

    }

}
