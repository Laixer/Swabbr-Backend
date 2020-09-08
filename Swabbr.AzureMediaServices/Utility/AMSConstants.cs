using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using System;

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

        internal const int VlogSasOutputExpireTimeMinutes = 30;

        internal const int LiveOutputArchiveWindowLengthHours = 24;

        internal const int PlaybackTokenTimeSubstractionMinutes = 5;

        internal const string ThumbnailDefaultIndex = "000001"; // TODO Clean up thumbnail index

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

        internal static Codec LivestreamAudioCodec() => new AacAudio(
                                channels: 2,
                                samplingRate: 48000,
                                bitrate: 128000,
                                profile: AacAudioProfile.AacLc);

        internal static Codec LivestreamVideoCodec() => new H264Video(
                                keyFrameInterval: TimeSpan.FromSeconds(2),
                                layers: new H264Layer[]
                                {
                                    new H264Layer (
                                        bitrate: 1000000,
                                        width: "1280",
                                        height: "720",
                                        label: "HD"
                                    )
                                });

        internal static Codec LivestreamThumbnailCodec() => new PngImage(
                                start: "25%",
                                layers: new PngLayer[]{
                                    new PngLayer(
                                        width: "50%",
                                        height: "50%"
                                    )
                                });
    }
}
