using Newtonsoft.Json;
using System;

namespace Swabbr.Api.ViewModels
{

    /// <summary>
    /// Represents a livestream playback entity.
    /// </summary>
    public sealed class LivestreamPlaybackOutputModel
    {

        /// <summary>
        /// Internal <see cref="Core.Entities.SwabbrUser"/> id of the person that is
        /// live.
        /// </summary>
        public Guid LiveUserId { get; set; }

        /// <summary>
        /// Internal <see cref="Core.Entities.Livestream"/> id.
        /// </summary>
        public Guid LiveLivestreamId { get; set; }

        /// <summary>
        /// Internal <see cref="Core.Entities.Vlog"/> id.
        /// </summary>
        public Guid LiveVlogId { get; set; }

        /// <summary>
        /// Endpoint to connect to.
        /// </summary>
        public Uri EndpointUrl { get; set; }

        /// <summary>
        /// Authentication token.
        /// </summary>
        public string Token { get; set; }

    }

}
