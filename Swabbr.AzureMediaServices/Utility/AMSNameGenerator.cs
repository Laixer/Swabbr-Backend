using System;

namespace Swabbr.AzureMediaServices.Utility
{

    /// <summary>
    /// Generates consistent names to be used in our Azure Media Services.
    /// </summary>
    internal static class AMSNameGenerator
    {

        internal static string LiveEventName() => $"live-event-{GenerateRandomId()}";

        internal static string VlogLiveOutputName(Guid correspondingVlogId) => $"vlog-live-output-{correspondingVlogId}";

        internal static string VlogLiveOutputAssetName(Guid correspondingVlogId) => $"vlog-asset-{correspondingVlogId}";

        internal static string VlogStreamingLocatorName(Guid correspondingVlogId) => $"vlog-streaming-locator-{correspondingVlogId}";

        internal static string ReactionInputAssetName(Guid reactionId) => $"reaction-input-{reactionId}";

        internal static string ReactionOutputAssetName(Guid reactionId) => $"reaction-asset-{reactionId}";

        internal static string ReactionJobName(Guid reactionId) => $"reaction-job-{reactionId}";

        internal static string ReactionVideoFileName(Guid reactionId) => $"reaction-{reactionId}";

        internal static string ReactionOutputContainerVideoFileName(Guid reactionId) => $"video-{ReactionVideoFileName(reactionId)}.mp4";

        internal static string ReactionOutputContainerThumbnailFileName(Guid reactionId) => $"thumbnail-{ReactionVideoFileName(reactionId)}-000001.png"; // TODO Fix

        internal static string OutputContainerMetadataFileNameRegex => @"^.+_metadata\.json$";

        private static string GenerateRandomId() => Nanoid.Nanoid.Generate(
                alphabet: "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ",
                size: 15);

    }

}
