using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels.Reaction
{

    /// <summary>
    /// Contains playback details for a <see cref="Core.Entities.Reaction"/>.
    /// </summary>
    public class ReactionPlaybackDetailsOutputModel
    {

        public Guid ReactionId { get; set; }

        public Uri EndpointUrl { get; set; }

        public string Token { get; set; }

    }

}
