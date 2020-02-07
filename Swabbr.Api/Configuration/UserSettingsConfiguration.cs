namespace Swabbr.Api.Configuration
{
    /// <summary>
    /// Configuration for user settings
    /// </summary>
    public class UserSettingsConfiguration
    {
        /// <summary>
        /// The maximum number of daily vlog requests a user can receive.
        /// </summary>
        public int DailyVlogRequestLimit { get; set; }
    }
}