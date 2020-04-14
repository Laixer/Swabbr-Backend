using Laixer.Utility.Extensions;
using System;

namespace Swabbr.AzureMediaServices.Utility
{

    /// <summary>
    /// Generates consistent names to be used in our Azure Media Services.
    /// </summary>
    internal static class AMSNameGenerator
    {

        internal static string LivestreamTransformName => "SwabbrLivestreamTransform";

        internal static string LivestreamTransformDescription => "Transform used for Swabbr Livestreams";

        internal static string ReactionTransformName => "SwabbrReactionTransform";

        internal static string ReactionTransformDescription => "Transform used for Swabbr Reactions";

        internal static string LivestreamDefaultName => "Swabbr Livestream";

        internal static string InputAssetName(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();
            return $"reaction-input-{reactionId}";
        }

        internal static string OutputAssetName(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();
            return $"reaction-output-{reactionId}";
        }

        internal static string LiveEventName()
        {
            return $"live-event-{GenerateRandomId()}";
        }

        internal static string StreamingLocatorName(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            return $"streaming-locator-{livestreamId}";
        }

        internal static string JobName(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();
            return $"reaction-job-{reactionId}";
        }

        internal static string VideoFileName(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();
            return $"reaction-{reactionId}";
        }

        internal static string OutputContainerVideoFileName(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();
            return $"video-{VideoFileName(reactionId)}.mp4";
        }

        internal static string OutputContainerThumbnailFileName(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();
            return $"thumbnail-{VideoFileName(reactionId)}-000001.png";
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
