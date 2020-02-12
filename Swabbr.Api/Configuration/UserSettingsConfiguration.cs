namespace Swabbr.Api.Configuration
{
    /// <summary>
    /// Configuration for user settings
    /// </summary>
    /// TODO THOMAS Why is this not in the core?
    public class UserSettingsConfiguration
    {
        /// <summary>
        /// The maximum number of daily vlog requests a user can receive.
        /// </summary>
        public int DailyVlogRequestLimit { get; set; }
    }
}