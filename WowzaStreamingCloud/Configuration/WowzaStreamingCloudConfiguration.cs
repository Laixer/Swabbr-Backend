namespace WowzaStreamingCloud.Configuration
{

    public class WowzaStreamingCloudConfiguration
    {
        /// <summary>
        /// WSC API host URL
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Version of the API
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// WSC API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// WSC Access key
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// WSC specific broadcast location
        /// </summary>
        public string BroadcastLocation { get; set; }

        /// <summary>
        /// Stream resolution width
        /// </summary>
        public int AspectRatioWidth { get; set; }

        /// <summary>
        /// Stream resolution height
        /// </summary>
        public int AspectRatioHeight { get; set; }
    }
}
