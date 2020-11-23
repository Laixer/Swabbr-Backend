using System;

namespace Swabbr.Core.Notifications.JsonWrappers
{
    /// <summary>
    ///     JSON wrapper for a vlog record request.
    /// </summary>
    public sealed class ParametersRecordVlog : ParametersJsonBase
    {
        /// <summary>
        ///     Moment when the vlogging request
        ///     was sent and thus when the timeout starts.
        /// </summary>
        public DateTimeOffset RequestMoment { get; set; }

        /// <summary>
        ///     Indicates how long the request is valid.
        /// </summary>
        public TimeSpan RequestTimeout { get; set; }

        /// <summary>
        ///     Internal not-yet-existing vlog id.
        /// </summary>
        /// <remarks>
        ///     This is a suggestion, the id does not actually
        ///     exist in our data store when this is sent.
        /// </remarks>
        public Guid VlogId { get; set; }
    }
}
