namespace Swabbr.Core.Configuration
{

    /// <summary>
    /// Contains <see cref="Swabbr"/> configuration parameters.
    /// </summary>
    public sealed class SwabbrConfiguration
    {

        /// <summary>
        /// Maximum reaction length.
        /// </summary>
        public uint ReactionLengthMaxInSeconds { get; set; }

        /// <summary>
        /// Minimum vlog length.
        /// </summary>
        public uint VlogLengthMinSeconds { get; set; }

        /// <summary>
        /// Maximum vlog length.
        /// </summary>
        public uint VlogLengthMaxSeconds { get; set; }

        /// <summary>
        /// Timeout in minutes before a vlog record request times out.
        /// </summary>
        public uint VlogRequestTimeoutMinutes { get; set; }

        /// <summary>
        /// Maximum daily vlog request count.
        /// </summary>
        public uint DailyVlogRequestLimit { get; set; }

        /// <summary>
        /// Minute of the day that our vlog requests start.
        /// </summary>
        public uint VlogRequestStartTimeMinutes { get; set; }

        /// <summary>
        /// Minute of the day that our vlog requests end.
        /// </summary>
        public uint VlogRequestEndTimeMinutes { get; set; }

        /// <summary>
        /// Timeout in seconds that the user has to connect to the livestream after
        /// notifying the backend that he or she will start streaming.
        /// </summary>
        public uint UserConnectTimeoutSeconds { get; set; }

    }

}
