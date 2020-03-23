namespace Swabbr.Infrastructure.Configuration
{

    /// <summary>
    /// Configuration file for Azure Notification Hub.
    /// </summary>
    public class NotificationHubConfiguration
    {

        /// <summary>
        /// Name of the connection string in the ConnectionString section in our configuration file.
        /// </summary>
        public string ConnectionStringName { get; set; }

        /// <summary>
        /// Name of the Hub in ANH.
        /// </summary>
        public string HubName { get; set; }

    }

}
