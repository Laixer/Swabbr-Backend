namespace Swabbr.WowzaStreamingCloud.Services
{

    /// <summary>
    /// Contains constants for managing our wowza livestreams.
    /// </summary>
    internal static class WowzaLivestreamingServiceConstants
    {

        internal const string BillingMode = "pay_as_you_go";
        internal const string ClosedCaptionType = "none";
        internal const string DeliveryMethod = "push";
        internal const string Encoder = "wowza_gocoder";
        internal const string PlayerType = "wowza_player";
        internal const string TranscoderType = "transcoded";

        internal const bool HostedPage = true;
        internal const bool HostedPageSharingIcons = true;
        internal const bool LowLatency = true;
        internal const bool PlayerResponsive = true;
        internal const bool Recording = true;

    }

}
