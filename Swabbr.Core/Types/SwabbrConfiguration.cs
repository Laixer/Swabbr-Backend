using System;

namespace Swabbr.Core.Types
{

    /// <summary>
    /// Contains <see cref="Swabbr"/> configuration parameters.
    /// TODO <see cref="uint"/>?
    /// </summary>
    public sealed class SwabbrConfiguration
    {

        /// <summary>
        /// Minimum vlog length.
        /// </summary>
        public int VlogLengthMinSeconds { get; set; } = 10;

        /// <summary>
        /// Maximum vlog length.
        /// </summary>
        public int VlogLengthMaxSeconds { get; set; } = 600;

        /// <summary>
        /// Timeout in seconds before a vlog record request times out.
        /// </summary>
        public int VlogRequestTimeoutMinutes { get; set; } = 5;
        
        /// <summary>
        /// Maximum daily vlog request count.
        /// </summary>
        public int DailyVlogRequestLimit { get; set; } = 3;

        public int VlogRequestStartTimeMinutes { get; set; } = (int)TimeSpan.FromHours(9).TotalMinutes;

        public int VlogRequestEndTimeMinutes { get; set; } = (int)TimeSpan.FromHours(22).TotalMinutes;

    }

}
