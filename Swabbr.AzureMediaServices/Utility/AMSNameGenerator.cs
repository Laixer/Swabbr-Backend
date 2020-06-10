using Laixer.Utility.Extensions;
using Swabbr.Core.Utility;
using System;

namespace Swabbr.AzureMediaServices.Utility
{
    /// <summary>
    /// Generates consistent names to be used in our Azure Media Services.
    /// </summary>
    public static class AMSNameGenerator
    {
        internal static string LiveEventName => $"live-event-{RandomIdGenerator.Generate(AMSConstants.LiveEventRandomIdLength)}";

        internal static string VlogLiveOutputName(Guid correspondingVlogId) => $"vlog-live-output-{correspondingVlogId}";

        internal static string VlogLiveOutputAssetName(Guid correspondingVlogId) => $"vlog-asset-{correspondingVlogId}";

        internal static string VlogStreamingLocatorName(Guid correspondingVlogId) => $"vlog-streaming-locator-{correspondingVlogId}";

        internal static string ReactionInputAssetName(Guid reactionId) => $"reaction-input-{reactionId}";

        internal static string ReactionOutputAssetName(Guid reactionId) => $"{AMSConstants.ReactionAssetPrefix}-{reactionId}";

        /// <summary>
        /// Extracts the internal reaction id from an asset name. This is the inverse
        /// of <see cref="ReactionOutputAssetName(Guid)"/>.
        /// </summary>
        /// <param name="reactionAssetName"></param>
        /// <returns>Internal reaction id</returns>
        public static Guid ReactionOutputAssetNameInverted(string reactionAssetName)
        {
            reactionAssetName.ThrowIfNullOrEmpty();
#pragma warning disable CA1062 // Validate arguments of public methods (static analyzer doesn't recognize custom throw function)
            if (reactionAssetName.Length != AMSConstants.ReactionAssetPrefix.Length + GuidExtensions.GuidAsStringLength) { throw new FormatException("Asset name has invalid length"); }
#pragma warning restore CA1062 // Validate arguments of public methods
            if (!reactionAssetName.StartsWith($"{AMSConstants.ReactionAssetPrefix}-", StringComparison.InvariantCulture)) { throw new FormatException($"Asset name doesnt start with {AMSConstants.ReactionAssetPrefix}"); }
            return Guid.TryParse(reactionAssetName.Substring(15), out var result) ? result : throw new FormatException("Couldn't parse asset name to get asset name id");
        }

        internal static string ReactionStreamingLocatorName(Guid reactionId) => $"reaction-streaming-locator-{reactionId}";

        internal static string ReactionJobName(Guid reactionId) => $"reaction-job-{reactionId}";

        internal static string ReactionVideoFileName(Guid reactionId) => $"reaction-{reactionId}";

        internal static string ReactionOutputContainerVideoFileName(Guid reactionId) => $"video-{ReactionVideoFileName(reactionId)}.mp4";

        internal static string ReactionOutputContainerThumbnailFileName(Guid reactionId) => $"thumbnail-{ReactionVideoFileName(reactionId)}-{AMSConstants.ThumbnailDefaultIndex}.png"; // TODO Fix
    }
}
