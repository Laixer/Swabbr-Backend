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
        public int VlogLengthMinSeconds { get; set; }

        /// <summary>
        /// Maximum vlog length.
        /// </summary>
        public int VlogLengthMaxSeconds { get; set; }

        /// <summary>
        /// Timeout in seconds before a vlog record request times out.
        /// </summary>
        public int VlogRequestTimeoutMinutes { get; set; }

        /// <summary>
        /// Maximum daily vlog request count.
        /// </summary>
        public int DailyVlogRequestLimit { get; set; }

        public int VlogRequestStartTimeMinutes { get; set; }

        public int VlogRequestEndTimeMinutes { get; set; }

    }

}
