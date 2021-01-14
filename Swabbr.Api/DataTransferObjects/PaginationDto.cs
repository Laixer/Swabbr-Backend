namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for our navigation.
    /// </summary>
    public record PaginationDto
    {
        /// <summary>
        ///     The page to fetch.
        /// </summary>
        public uint Offset { get; init; }

        /// <summary>
        ///     The amount of items to fetch.
        /// </summary>
        public uint Limit { get; init; }
    }
}
