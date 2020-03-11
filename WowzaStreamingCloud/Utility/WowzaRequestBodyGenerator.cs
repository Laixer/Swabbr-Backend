using Swabbr.WowzaStreamingCloud.Configuration;
using Swabbr.WowzaStreamingCloud.Entities.Livestreams;
using System;
using static Swabbr.WowzaStreamingCloud.Utility.WowzaConstants;

namespace Swabbr.WowzaStreamingCloud.Utility
{

    /// <summary>
    /// Wrapper to create wowza request bodies in an elegant manner.
    /// </summary>
    internal static class RequestBodyGenerator
    {

        internal static WscCreateLivestreamRequest CreateLivestream(WowzaStreamingCloudConfiguration wscOptions)
        {
            if (wscOptions == null) { throw new ArgumentNullException(nameof(wscOptions)); }

            return new WscCreateLivestreamRequest
            {
                Livestream = new WscCreateLiveStreamRequestBody
                {
                    AspectRatioWidth = wscOptions.AspectRatioWidth,
                    AspectRatioHeight = wscOptions.AspectRatioHeight,
                    BroadcastLocation = wscOptions.BroadcastLocation,
                    Name = "Swabbr Livestream", // TODO Do we want to do anything with this?
                    BillingMode = BillingMode,
                    ClosedCaptionType = ClosedCaptionType,
                    DeliveryMethod = DeliveryMethod,
                    Encoder = WowzaConstants.Encoder, // TODO Why explicit?
                    PlayerType = PlayerType,
                    TranscoderType = TranscoderType,
                    HostedPage = HostedPage,
                    HostedPageSharingIcons = HostedPageSharingIcons,
                    LowLatency = LowLatency,
                    PlayerResponsive = PlayerResponsive,
                    Recording = Recording,
                }
            };
        }

    }
}
