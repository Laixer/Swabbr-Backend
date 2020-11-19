using Swabbr.Core.Types;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for handling everything that is related to video playback.
    /// </summary>
    public interface IPlaybackService
    {
        /// <summary>
        ///     Get playback details for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to watch.</param>
        /// <param name="watchingUserId">The user that watches.</param>
        /// <returns>Playback details.</returns>
        Task<VlogPlaybackDetails> GetVlogDownstreamParametersAsync(Guid vlogId, Guid watchingUserId);

        /// <summary>
        ///     Get playback details for a reaction.
        /// </summary>
        /// <param name="reactionId">The reaction to watch.</param>
        /// <param name="watchingUserId">The user that watches.</param>
        /// <returns>Playback details.</returns>
        Task<ReactionPlaybackDetails> GetReactionDownstreamParametersAsync(Guid reactionId, Guid watchingUserId);
    }
}
