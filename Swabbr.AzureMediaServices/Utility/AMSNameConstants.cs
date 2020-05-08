namespace Swabbr.AzureMediaServices.Utility
{

    /// <summary>
    /// Contains constant name variables for usage in Azure Media Services.
    /// </summary>
    internal static class AMSNameConstants
    {

        internal static string LivestreamTransformName => "SwabbrLivestreamTransform";

        internal static string LivestreamTransformDescription => "Transform used for Swabbr Livestreams";

        internal static string ReactionTransformName => "SwabbrReactionTransform";

        internal static string ReactionTransformDescription => "Transform used for Swabbr Reactions";

        internal static string LivestreamDefaultName => "Swabbr Livestream";

        internal static string StreamingEndpointName => "default"; // TODO "swabbr-streamingendpoint";

        internal static string StreamingPolicyName => "swabbr-streamingpolicy";

        internal static string ContentKeyPolicyName => "swabbr-contentkeypolicy";

        internal static string ContentKeyPolicyOptionName => "swabbr-contentkeypolicyoption";

    }

}
