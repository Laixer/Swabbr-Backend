namespace Swabbr.Core
{
    /// <summary>
    ///     Contains swabbr configuration parameters.
    /// </summary>
    public sealed class SwabbrConfiguration
    {
        /// <summary>
        ///     Maximum reaction length.
        /// </summary>
        public uint ReactionLengthMaxInSeconds { get; set; }

        /// <summary>
        ///     Minimum vlog length.
        /// </summary>
        public uint VlogLengthMinSeconds { get; set; }

        /// <summary>
        ///     Maximum vlog length.
        /// </summary>
        public uint VlogLengthMaxSeconds { get; set; }

        /// <summary>
        ///     Timeout in minutes before a vlog record request times out.
        /// </summary>
        public uint VlogRequestTimeoutMinutes { get; set; }

        /// <summary>
        ///     Maximum daily vlog request count.
        /// </summary>
        public uint MaxDailyVlogRequestLimit { get; set; }

        /// <summary>
        ///     Minute of the day that our vlog requests start.
        /// </summary>
        /// <remarks>
        ///     This is not timezone dependent.
        /// </remarks>
        public uint VlogRequestStartTimeMinutes { get; set; }

        /// <summary>
        ///     Minute of the day that our vlog requests end.
        /// </summary>
        /// <remarks>
        ///     This is not timezone dependent.
        /// </remarks>
        public uint VlogRequestEndTimeMinutes { get; set; }
    }
}
