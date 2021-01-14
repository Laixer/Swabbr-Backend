namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for our navigation.
    /// </summary>
    public class PaginationDto
    {
        /// <summary>
        ///     The page to fetch.
        /// </summary>
        public uint Offset { get; set; }

        /// <summary>
        ///     The amount of items to fetch.
        /// </summary>
        public uint Limit { get; set; }
    }
}
