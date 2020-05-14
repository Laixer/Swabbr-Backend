namespace Swabbr.Core.Configuration
{

    /// <summary>
    /// Contains <see cref="Swabbr"/> configuration parameters.
    /// TODO <see cref="uint"/>?
    /// </summary>
    public sealed class SwabbrConfiguration
    {

        /// <summary>
        /// Maximum reaction length.
        /// </summary>
        public int ReactionLengthMaxInSeconds { get; set; }

        /// <summary>
        /// Minimum vlog length.
        /// </summary>
        public int VlogLengthMinSeconds { get; set; }

        /// <summary>
        /// Maximum vlog length.
        /// </summary>
        public int VlogLengthMaxSeconds { get; set; }

        /// <summary>
        /// Timeout in minutes before a vlog record request times out.
        /// </summary>
        public int VlogRequestTimeoutMinutes { get; set; }

        /// <summary>
        /// Maximum daily vlog request count.
        /// </summary>
        public int DailyVlogRequestLimit { get; set; }

        /// <summary>
        /// Minute of the day that our vlog requests start.
        /// </summary>
        public int VlogRequestStartTimeMinutes { get; set; }

        /// <summary>
        /// Minute of the day that our vlog requests end.
        /// </summary>
        public int VlogRequestEndTimeMinutes { get; set; }

        /// <summary>
        /// Timeout in seconds that the user has to connect to the livestream after
        /// notifying the backend that he or she will start streaming.
        /// </summary>
        public int UserConnectTimeoutSeconds { get; set; }

    }

}
