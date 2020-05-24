namespace Swabbr.AzureFunctions.Types
{

    /// <summary>
    /// JSON wrapper for when the user vlog time has expired.
    /// </summary>
    public sealed class VlogTimeExpiredWrapper
    {

        public string LivestreamExternalId { get; set; }

    }

}
