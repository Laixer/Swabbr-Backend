namespace Swabbr.AzureMediaServices.Utility
{
    /// <summary>
    /// Contains constant name variables for usage in Azure Media Services.
    /// </summary>
    internal static class AMSConstants
    {
        internal const int LiveEventRandomIdLength = 15;

        internal const int ReactionSasSubstractTimeMinutes = 5;

        internal const int ReactionSasInputExpireTimeMinutes = 15;
        
        internal const int ReactionSasOutputExpireTimeMinutes = 30;

        internal const int LiveOutputArchiveWindowLengthHours = 24;

        internal const string ThumbnailDefaultIndex = "000001"; // TODO This should be cleaned up

        internal const string ReactionAssetPrefix = "reaction-asset";

        internal const string LivestreamTransformName = "SwabbrLivestreamTransform";

        internal const string LivestreamTransformDescription = "Transform used for Swabbr Livestreams";

        internal const string ReactionTransformName = "SwabbrReactionTransform";

        internal const string ReactionTransformDescription = "Transform used for Swabbr Reactions";

        internal const string LivestreamDefaultName = "Swabbr Livestream";

        internal const string StreamingEndpointName = "default";

        internal const string StreamingPolicyName = "swabbr-streamingpolicy";

        internal const string ContentKeyPolicyName = "swabbr-contentkeypolicy";

        internal const string ContentKeyPolicyOptionName = "swabbr-contentkeypolicyoption";

        internal const string FormatVideoFileNamePattern = "video-{Basename}{Extension}";

        internal const string FormatThumbnailFileNamePattern = "thumbnail-{Basename}-{Index}{Extension}";

        internal const string OutputContainerMetadataFileNameRegex = @"^.+_metadata\.json$";
    }
}
