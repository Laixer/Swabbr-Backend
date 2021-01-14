namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for statistics of a data set.
    /// </summary>
    public record DatasetStatsDto
    {
        /// <summary>
        ///     Total items in the data set.
        /// </summary>
        public uint Count { get; init; }
    }
}
