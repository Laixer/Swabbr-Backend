namespace Swabbr.Core.Types
{

    /// <summary>
    /// Contains <see cref="Swabbr"/> configuration parameters.
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
        public int VlogRequestTimeoutSeconds { get; set; } = 300;

        /// <summary>
        /// Maximum daily vlog request count.
        /// </summary>
        public int DailyVlogRequestLimit { get; set; } = 3;

    }

}
