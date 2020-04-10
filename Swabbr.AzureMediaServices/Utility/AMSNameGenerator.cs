using Laixer.Utility.Extensions;
using System;

namespace Swabbr.AzureMediaServices.Utility
{

    /// <summary>
    /// Generates consistent names to be used in our Azure Media Services.
    /// </summary>
    internal static class AMSNameGenerator
    {

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

        internal static string JobName(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();
            return $"reaction-job-{reactionId}";
        }

        internal static string TransformName => "MyDebugTransform";

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

    }

}
