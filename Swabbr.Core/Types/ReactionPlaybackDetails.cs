using System;

namespace Swabbr.Core.Types
{

    /// <summary>
    /// Contains the details we need to playback a <see cref="Entities.Reaction"/>.
    /// </summary>
    public sealed class ReactionPlaybackDetails
    {

        public Guid ReactionId { get; set; }

        public Uri EndpointUrl { get; set; }

        public string Token { get; set; }

    }

}
