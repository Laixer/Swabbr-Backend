using System;

namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Represents the playback details for a single livestream.
    /// </summary>
    public sealed class LivestreamDownstreamDetails
    {
        /// <summary>
        ///     Internal <see cref="Entities.SwabbrUser"/> id of 
        ///     the person that is live.
        /// </summary>
        public Guid LiveUserId { get; set; }

        /// <summary>
        ///     Internal <see cref="Entities.Livestream"/> id.
        /// </summary>
        public Guid LiveLivestreamId { get; set; }

        /// <summary>
        ///     Internal <see cref="Entities.Vlog"/> id.
        /// </summary>
        public Guid LiveVlogId { get; set; }

        /// <summary>
        ///     Endpoint to connect to.
        /// </summary>
        public Uri EndpointUrl { get; set; }

        /// <summary>
        ///     Authentication token.
        /// </summary>
        public string Token { get; set; }
    }
}
