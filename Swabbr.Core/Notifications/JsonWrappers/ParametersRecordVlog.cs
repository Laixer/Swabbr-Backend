using System;

namespace Swabbr.Core.Notifications.JsonWrappers
{

    /// <summary>
    /// Contains parameters we need to be able to start streaming to a livestream.
    /// </summary>
    public sealed class ParametersRecordVlog : ParametersJsonBase
    {

        /// <summary>
        /// Moment when the request was sent.
        /// </summary>
        public DateTimeOffset RequestMoment { get; set; }

        /// <summary>
        /// Indicates how long the request is valid.
        /// </summary>
        public TimeSpan RequestTimeout { get; set; }

        /// <summary>
        /// Internal <see cref="Entities.Livestream"/> id.
        /// </summary>
        public Guid LivestreamId { get; set; }

        /// <summary>
        /// Internal <see cref="Entities.Vlog"/> id.
        /// </summary>
        public Guid VlogId { get; set; }

    }

}
