using System;

namespace Swabbr.Core.Types
{

    /// <summary>
    /// Contains the details we need to playback a <see cref="Core.Entities.Vlog"/>.
    /// </summary>
    public sealed class VlogPlaybackDetails
    {

        public Guid VlogId { get; set; }

        public Uri EndpointUrl { get; set; }

        public string Token { get; set; }

    }

}
