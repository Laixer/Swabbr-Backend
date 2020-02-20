using System;

namespace Swabbr.Core.Types
{

    /// <summary>
    /// Represents the playback details for a single livestream.
    /// </summary>
    public sealed class LivestreamPlaybackDetails
    {

        /// <summary>
        /// Streaming playback URL
        /// </summary>
        public Uri PlaybackUrl { get; set; }

    }

}
