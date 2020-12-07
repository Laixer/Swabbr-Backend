namespace Swabbr.Infrastructure.Configuration
{
    /// <summary>
    ///     Configuration file for Azure Notification Hub.
    /// </summary>
    public class NotificationHubConfiguration
    {
        /// <summary>
        ///     Connection string name.
        /// </summary>
        public string ConnectionStringName { get; set; }

        /// <summary>
        ///     Name of the Hub in Azure Notification Hub.
        /// </summary>
        public string HubName { get; set; }
    }
}
