using System;

namespace Swabbr.AzureMediaServices.Utility
{

    /// <summary>
    /// Generates consistent names to be used in our Azure Media Services.
    /// </summary>
    internal static class AMSNameGenerator
    {

        internal static string LiveEventName()
        {
            return $"live-event-{GenerateRandomId()}";
        }

        internal static string VlogLiveOutputName(Guid correspondingVlogId)
        {
            return $"vlog-live-output-{correspondingVlogId}";
        }

        internal static string VlogLiveOutputAssetName(Guid correspondingVlogId)
        {
            return $"vlog-asset-{correspondingVlogId}";
        }

        internal static string VlogStreamingLocatorName(Guid correspondingVlogId)
        {
            return $"vlog-streaming-locator-{correspondingVlogId}";
        }

        internal static string ReactionInputAssetName(Guid reactionId)
        {
            return $"reaction-input-{reactionId}";
        }

        internal static string ReactionOutputAssetName(Guid reactionId)
        {
            return $"reaction-asset-{reactionId}";
        }

        internal static string ReactionStreamingLocatorName(Guid reactionId)
        {
            return $"reaction-streaming-locator-{reactionId}";
        }

        internal static string ReactionJobName(Guid reactionId)
        {
            return $"reaction-job-{reactionId}";
        }

        internal static string ReactionVideoFileName(Guid reactionId)
        {
            return $"reaction-{reactionId}";
        }

        internal static string ReactionOutputContainerVideoFileName(Guid reactionId)
        {
            return $"video-{ReactionVideoFileName(reactionId)}.mp4";
        }

        internal static string ReactionOutputContainerThumbnailFileName(Guid reactionId)
        {
            return $"thumbnail-{ReactionVideoFileName(reactionId)}-000001.png"; // TODO Fix
        }

        internal static string OutputContainerMetadataFileNameRegex => @"^.+_metadata\.json$";

        private static string GenerateRandomId()
        {
            return Nanoid.Nanoid.Generate(
alphabet: "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ",
size: 15);
        }
    }

}
