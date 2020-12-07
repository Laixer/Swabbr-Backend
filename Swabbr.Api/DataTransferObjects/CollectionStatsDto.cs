namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for statistics about a collection.
    /// </summary>
    public class CollectionStatsDto
    {
        /// <summary>
        ///     Total items in the collection.
        /// </summary>
        public uint Count { get; set; }
    }
}
