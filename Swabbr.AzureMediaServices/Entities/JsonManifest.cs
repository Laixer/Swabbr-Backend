namespace Swabbr.AzureMediaServices.Entities
{

    /// <summary>
    /// Representation of 
    /// </summary>
    internal sealed class JsonManifest
    {
        public JsonManifestAssetFile[] AssetFile { get; set; }
    }

    internal sealed class JsonManifestAssetFile
    {
        public JsonManifestVideoTrack[] VideoTracks { get; set; }
    }

    internal sealed class JsonManifestVideoTrack
    {
        public double FrameRate { get; set; }
        public int NumberOfFrames { get; set; }
    }

}
